using PoeLib.JSON;
using PoeLib;
using System.Collections.Concurrent;
using PoeLib.Trade;
using PoeTradeMonitor.GUI.Interfaces;
using PoeLib.Settings;
using System.IO;
using System.Text;
using System.Globalization;
using System.Timers;
using Microsoft.Extensions.Logging;

namespace PoeTradeMonitor.GUI.Services;

public class PoePriceChecker : IPoePriceChecker, IDisposable
{
    private readonly ILogger<PoePriceChecker> logger;
    private readonly IPoeItemSearch poeItemSearch;
    private readonly ISettingsManager settingsManager;
    private ConcurrentDictionary<string, decimal> itemPriceDictionary = new();

    // Store price history for each item to calculate moving average
    private ConcurrentDictionary<string, Queue<decimal>> itemPriceHistory = new();
    private const int MovingAverageWindow = 24; // 2 hours

    private System.Timers.Timer updateTimer;
    private readonly string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PoeTradeMonitor", "ItemPrices.csv");
    private bool isLogging = false;

    private (string name, PoeItemSearchRequest searchRequest)[] monitoredItems =
    [
        ("Helical Ring", new PoeItemSearchRequest {
            query = new Query{
                status = new Status{
                    option = "online"
                },
                type = "Helical Ring",
                stats = [
                    new Stat{
                        type = "not",
                        filters = [
                            new StatFilter {
                                id = "pseudo.pseudo_has_influence_count",
                                disabled = false
                            }
                        ],
                        disabled = false
                    }
                ],
                filters = new Filters {
                    misc_filters = new MiscFilters {
                        filters = new FilterTypes {
                            ilvl = new Ilvl {
                                min = 84
                            },
                            mirrored = new Mirrored {
                                option = false
                            },
                            corrupted = new Corrupted {
                                option = false
                            },
                            fractured_item = new FracturedItem {
                                option = false
                            }
                        },
                        disabled = false
                    },
                    trade_filters = new TradeFilters {
                        filters = new FilterTypes {
                            price = new Price {
                                option = "divine"
                            }
                        },
                        disabled = false
                    }
                }
            },
            sort = new Sort {
                price = "asc"
            }
        }),
        ("Focused Amulet", new PoeItemSearchRequest {
            query = new Query{
                status = new Status{
                    option = "online"
                },
                type = "Focused Amulet",
                stats = [
                    new Stat{
                        type = "not",
                        filters = [
                            new StatFilter {
                                id = "pseudo.pseudo_has_influence_count",
                                disabled = false
                            }
                        ],
                        disabled = false
                    }
                ],
                filters = new Filters {
                    misc_filters = new MiscFilters {
                        filters = new FilterTypes {
                            ilvl = new Ilvl {
                                min = 84
                            },
                            mirrored = new Mirrored {
                                option = false
                            },
                            corrupted = new Corrupted {
                                option = false
                            },
                            fractured_item = new FracturedItem {
                                option = false
                            }
                        },
                        disabled = false
                    },
                    trade_filters = new TradeFilters {
                        filters = new FilterTypes {
                            price = new Price {
                                option = "divine"
                            }
                        },
                        disabled = false
                    }
                }
            },
            sort = new Sort {
                price = "asc"
            }
        }),
        ("Simplex Amulet", new PoeItemSearchRequest {
            query = new Query{
                status = new Status{
                    option = "online"
                },
                type = "Simplex Amulet",
                stats = [
                    new Stat{
                        type = "not",
                        filters = [
                            new StatFilter {
                                id = "pseudo.pseudo_has_influence_count",
                                disabled = false
                            }
                        ],
                        disabled = false
                    }
                ],
                filters = new Filters {
                    misc_filters = new MiscFilters {
                        filters = new FilterTypes {
                            ilvl = new Ilvl {
                                min = 84
                            },
                            mirrored = new Mirrored {
                                option = false
                            },
                            corrupted = new Corrupted {
                                option = false
                            },
                            fractured_item = new FracturedItem {
                                option = false
                            }
                        },
                        disabled = false
                    },
                    trade_filters = new TradeFilters {
                        filters = new FilterTypes {
                            price = new Price {
                                option = "divine"
                            }
                        },
                        disabled = false
                    }
                }
            },
            sort = new Sort {
                price = "asc"
            }
        }),
    ];

    public PoePriceChecker(ILogger<PoePriceChecker> logger, IPoeItemSearch poeItemSearch, ISettingsManager settingsManager)
    {
        this.logger = logger;
        this.poeItemSearch = poeItemSearch;
        this.settingsManager = settingsManager;

        updateTimer = new System.Timers.Timer(TimeSpan.FromMinutes(5));
        updateTimer.Elapsed += OnTimerElapsed;
        updateTimer.AutoReset = true;
        updateTimer.Start();

        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)!);

        if (!File.Exists(logFilePath))
        {
            CreateCsvHeader();
        }

        // Initialize the price history queues for each monitored item
        InitializePriceHistory();
    }

    private void InitializePriceHistory()
    {
        foreach ((var itemName, var searchRequest) in monitoredItems)
        {
            var realItemName = string.IsNullOrEmpty(itemName) ? searchRequest.query.type : itemName;
            itemPriceHistory[realItemName] = new Queue<decimal>(MovingAverageWindow);
        }
    }

    public void Start()
    {
        if (!isLogging)
        {
            isLogging = true;
        }
    }

    public void Stop()
    {
        if (isLogging)
        {
            isLogging = false;
        }
    }

    public decimal GetPrice(string itemName)
    {
        if (itemPriceDictionary.TryGetValue(itemName, out var price))
            return price;
        return 0;
    }

    private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        try
        {
            await UpdatePricesAndLogAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update item prices");
        }
    }

    public async Task UpdatePricesAndLogAsync()
    {
        await UpdatePrices();
        if (isLogging)
            LogPricesToCsv();
    }

    private async Task UpdatePrices()
    {
        foreach ((var itemName, var searchRequest) in monitoredItems)
        {
            var realItemName = string.IsNullOrEmpty(itemName) ? searchRequest.query.type : itemName;
            try
            {
                var response = await poeItemSearch.SearchAsync(settingsManager.Settings.League, searchRequest);
                if (response.result.Any())
                {
                    var results = await poeItemSearch.FetchItemResults(response.result);
                    var currentPrice = CalculateCurrentPrice(results.result);

                    // Update price history and calculate moving average
                    UpdatePriceHistory(realItemName, currentPrice);
                    var currentAvgPrice = CalculateMovingAverage(realItemName);
                    logger.LogDebug($"Current price for {realItemName}: {currentPrice} divine, MovingAvg: {currentAvgPrice} divine");
                    itemPriceDictionary[realItemName] = currentAvgPrice;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating price for {realItemName}: {ex.Message}");
            }
            await Task.Delay(5000);
        }
    }

    private void UpdatePriceHistory(string itemName, decimal currentPrice)
    {
        if (!itemPriceHistory.TryGetValue(itemName, out var priceQueue))
        {
            priceQueue = new Queue<decimal>(MovingAverageWindow);
            itemPriceHistory[itemName] = priceQueue;
        }

        // If we've reached the window size, remove the oldest price
        if (priceQueue.Count >= MovingAverageWindow)
        {
            priceQueue.Dequeue();
        }

        // Add the new price
        priceQueue.Enqueue(currentPrice);
    }

    private decimal CalculateMovingAverage(string itemName)
    {
        if (itemPriceHistory.TryGetValue(itemName, out var priceQueue) && priceQueue.Count > 0)
        {
            return Math.Round(priceQueue.Average(), 2);
        }
        return 0;
    }

    private void CreateCsvHeader()
    {
        using var writer = new StreamWriter(logFilePath, false, Encoding.UTF8);
        var headerBuilder = new StringBuilder("Timestamp");

        foreach ((var itemName, var searchRequest) in monitoredItems)
        {
            var realItemName = string.IsNullOrEmpty(itemName) ? searchRequest.query.type : itemName;
            headerBuilder.Append($",{realItemName}");
        }

        writer.WriteLine(headerBuilder.ToString());
    }

    private void LogPricesToCsv()
    {
        try
        {
            // Check if file exists, if not create it with headers
            if (!File.Exists(logFilePath))
            {
                CreateCsvHeader();
            }

            // Format the timestamp in ISO 8601 format which Google Sheets recognizes well
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");

            // Append the new data row
            using var writer = new StreamWriter(logFilePath, true, Encoding.UTF8);
            var dataBuilder = new StringBuilder(timestamp);

            foreach ((var itemName, var searchRequest) in monitoredItems)
            {
                var realItemName = string.IsNullOrEmpty(itemName) ? searchRequest.query.type : itemName;
                if (itemPriceDictionary.TryGetValue(realItemName, out decimal price))
                {
                    dataBuilder.Append($",{price.ToString(CultureInfo.InvariantCulture)}");
                }
                else
                {
                    dataBuilder.Append(",");  // Empty column if price not available
                }
            }

            writer.WriteLine(dataBuilder.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error logging prices to CSV: {ex.Message}");
        }
    }

    private decimal CalculateCurrentPrice(List<PoeItemSearchResult> allItems)
    {
        if (allItems == null || !allItems.Any())
            return 0;

        // Filter out outliers
        var filteredItems = FilterOutliers(allItems);

        // Check for recent listings
        var recentFilteredItems = filteredItems.Where(item =>
        {
            TimeSpan timeDifference = DateTime.UtcNow - item.listing.indexed;
            return timeDifference.TotalHours <= 1;
        }).ToList();

        // If we have recent filtered items, take lowest (up to) 3 for average
        if (recentFilteredItems.Any())
        {
            return recentFilteredItems
                .First()
                .listing.price.amount;
        }

        // Fallback: Use the lowest priced non-outlier item
        return filteredItems
            .First()
            .listing.price.amount;
    }

    private List<PoeItemSearchResult> FilterOutliers(List<PoeItemSearchResult> sortedItems)
    {
        var filteredItems = new List<PoeItemSearchResult>();

        for (int i = 0; i < sortedItems.Count; i++)
        {
            // Find the next (up to) 5 higher priced items
            var nextFiveItems = sortedItems
                .Skip(i + 1)
                .Take(5)
                .ToList();

            // If there are no higher priced items to compare to, include this item
            if (!nextFiveItems.Any())
            {
                filteredItems.Add(sortedItems[i]);
                continue;
            }

            // Calculate the average of the next (up to) 5 higher priced items
            decimal nextFiveAverage = nextFiveItems.Average(item => item.listing.price.amount);

            // Include this item only if its price is at least 50% of the average of next items
            if (sortedItems[i].listing.price.amount >= nextFiveAverage * 0.5m)
            {
                filteredItems.Add(sortedItems[i]);
            }
        }

        return filteredItems;
    }

    public void Dispose()
    {
        updateTimer?.Stop();
        updateTimer?.Dispose();
    }
}