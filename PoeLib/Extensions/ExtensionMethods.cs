using PoeLib.Common;
using PoeLib.JSON.PoeNinja;
using PoeTrade.Contracts;
using PoeTrade.Contracts.Extensions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PoeLib.Extensions;

/// <summary>
/// Extension methods for various types used throughout the PoeLib library.
/// Provides utility functions for data analysis, string manipulation, and task management.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Determines if a Sparkline price history shows volatile price movement.
    /// </summary>
    /// <param name="history">The sparkline data to analyze.</param>
    /// <returns>True if price movement exceeds volatility thresholds; otherwise, false.</returns>
    public static bool Volatile(this Sparkline history)
    {
        var data = history.data;
        if (!data[data.Count - 1].HasValue || !data[data.Count - 2].HasValue || data[data.Count - 2]!.Value == 0 || data.Count < 2)
            return false;
        var delta = data[data.Count - 1]!.Value - data[data.Count - 2]!.Value;
        return delta < -20 || delta > 40;
    }

    /// <summary>
    /// Starts a task in an STA (Single Threaded Apartment) thread.
    /// </summary>
    /// <param name="task">The task to execute in STA mode.</param>
    /// <returns>A task representing the STA execution.</returns>
    public static Task StartSTATask(this Task task)
    {
        TaskCompletionSource<object> source = new TaskCompletionSource<object>();
        Thread thread = new Thread(() =>
        {
            try
            {
                task.Wait();
                source.SetResult(new object());
            }
            catch (Exception ex)
            {
                source.SetException(ex);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return source.Task;
    }

    /// <summary>
    /// Asynchronously waits for a WaitHandle to be signaled with timeout and cancellation support.
    /// </summary>
    /// <param name="handle">The WaitHandle to wait for.</param>
    /// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>True if the handle was signaled; false if timeout occurred.</returns>
    public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        RegisteredWaitHandle? registeredHandle = null;
        CancellationTokenRegistration tokenRegistration = default;
        try
        {
            var tcs = new TaskCompletionSource<bool>();
            registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                handle,
                (state, timedOut) => ((TaskCompletionSource<bool>)state!).TrySetResult(!timedOut),
                tcs,
                millisecondsTimeout,
                true);
            tokenRegistration = cancellationToken.Register(
                state => ((TaskCompletionSource<bool>)state!).TrySetCanceled(),
                tcs);
            return await tcs.Task;
        }
        finally
        {
            if (registeredHandle != null)
                registeredHandle.Unregister(null);
            tokenRegistration.Dispose();
        }
    }

    /// <summary>
    /// Asynchronously waits for a WaitHandle to be signaled with cancellation support (no timeout).
    /// </summary>
    /// <param name="handle">The WaitHandle to wait for.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>True if the handle was signaled; false if cancelled.</returns>
    public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken)
    {
        return handle.WaitOneAsync(Timeout.Infinite, cancellationToken);
    }

    /// <summary>
    /// Gets the maximum stack size for a specific currency type.
    /// </summary>
    /// <param name="currency">The currency type to get stack size for.</param>
    /// <returns>The maximum stack size for the currency.</returns>
    public static int GetCurrencyStackSize(this TradeCurrencyType currency)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        if (fi == null) return 0;
        var attributes = (StackSizeAttribute[])fi.GetCustomAttributes(typeof(StackSizeAttribute), false);

        return attributes.Length > 0 ? attributes[0].Size : 0;
    }

    /// <summary>
    /// Calculates the number of stacks required for the given currency amount.
    /// </summary>
    /// <param name="currencyInfo">The currency information to calculate stacks for.</param>
    /// <returns>The number of stacks needed to hold the currency amount.</returns>
    public static int GetCurrencyStatcks(this CurrencyInfo currencyInfo)
    {
        var stacks = (int)currencyInfo.Amount / currencyInfo.Type.GetCurrencyStackSize();
        var stackRemainder = (int)currencyInfo.Amount % currencyInfo.Type.GetCurrencyStackSize();
        return stackRemainder == 0 ? stacks : stacks + 1;
    }

    /// <summary>
    /// Converts a currency name string to its corresponding TradeCurrencyType enum.
    /// </summary>
    /// <param name="currencyString">The currency name string.</param>
    /// <returns>The matching TradeCurrencyType enum value.</returns>
    public static TradeCurrencyType GetCurrencyType(this string currencyString)
    {
        return ((TradeCurrencyType[]) Enum.GetValues(typeof(TradeCurrencyType))).FirstOrDefault(currencyType => currencyString.Equals(currencyType.GetCurrencyDescription(), StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Converts an object to JSON StringContent for HTTP requests.
    /// </summary>
    /// <param name="request">The object to serialize.</param>
    /// <param name="json">The resulting JSON string.</param>
    /// <returns>StringContent configured for JSON HTTP requests.</returns>
    public static StringContent ToStringContent(this object request, out string json)
    {
        var jsonSerializerOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        json = JsonSerializer.Serialize(request, jsonSerializerOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Converts the price to Divine Orbs based on the provided divine rate.
    /// </summary>
    /// <param name="divineRate">How many base currency units equal 1 Divine Orb (e.g., 120.0 means 120 base currency = 1 Divine)</param>
    /// <returns>The price converted to Divine Orbs</returns>
    public static decimal PriceInDivine(this PriceInfo price, decimal divineRate)
    {
        if (price.CurrencyType == TradeCurrencyType.Divine)
            return price.Amount;

        if (price.CurrencyType == Constants.BaseCurrencyType)
            return price.Amount / divineRate;

        // For other currencies, first convert to base currency, then to divine
        var priceInBase = price.PriceInBaseCurrency(divineRate);
        return priceInBase / divineRate;
    }

    /// <summary>
    /// Converts the price to the base currency type based on the provided divine rate.
    /// </summary>
    /// <param name="divineRate">How many base currency units equal 1 Divine Orb (e.g., 120.0 means 120 base currency = 1 Divine)</param>
    /// <returns>The price converted to base currency</returns>
    public static decimal PriceInBaseCurrency(this PriceInfo price, decimal divineRate)
    {
        if (price.CurrencyType == Constants.BaseCurrencyType)
            return price.Amount;

        if (price.CurrencyType == TradeCurrencyType.Divine)
            return price.Amount * divineRate;

        throw new NotSupportedException($"Conversion from {price.CurrencyType} to base currency is not supported without additional exchange rates.");
    }
}
