# PoeLib.Tests

Comprehensive test suite for the PoeLib core library, ensuring reliability and correctness of Path of Exile API integrations, price fetchers, and utility functions.

## 🎯 Purpose

This test project provides comprehensive coverage for:
- **Price Fetcher Validation**: Test PoE.ninja and PoEWatch API integrations
- **Message Parser Testing**: Validate chat message parsing and conversion
- **Cache Behavior**: Ensure currency and message caches work correctly
- **Notification Systems**: Test Pushover and other notification mechanisms
- **Exception Handling**: Verify error conditions and edge cases

## 📊 Test Coverage

### Core Components Tested

| Component | Test Coverage | Test Count |
|-----------|---------------|------------|
| **PoeNinjaWrapper** | API responses, error handling, rate limiting | 15+ tests |
| **PoeWatchWrapper** | Data parsing, league support, price validation | 12+ tests |
| **MessageParser** | Chat patterns, trade messages, edge cases | 20+ tests |
| **CurrencyPriceCache** | Caching logic, expiration, memory management | 10+ tests |
| **ChatMessageCache** | Message filtering, duplicate detection | 8+ tests |
| **PushoverNotification** | Message delivery, error handling | 6+ tests |
| **Exception Classes** | Custom exceptions, error information | 5+ tests |

## 🧪 Test Categories

### API Integration Tests

```csharp
[TestClass]
public class PoeNinjaWrapperTests
{
    private Mock<HttpClient> _mockHttpClient;
    private PoeNinjaWrapper _wrapper;
    
    [TestInitialize]
    public void Setup()
    {
        _mockHttpClient = new Mock<HttpClient>();
        _wrapper = new PoeNinjaWrapper(_mockHttpClient.Object);
    }
    
    [TestMethod]
    public async Task GetCurrencyData_ValidLeague_ReturnsData()
    {
        // Arrange
        var mockResponse = LoadTestData("poe-ninja-currency-response.json");
        SetupHttpResponse(_mockHttpClient, mockResponse);
        
        // Act
        var result = await _wrapper.GetCurrencyDataAsync("Standard");
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Assert.IsTrue(result.All(c => !string.IsNullOrEmpty(c.CurrencyTypeName)));
    }
    
    [TestMethod]
    public async Task GetCurrencyData_InvalidLeague_ThrowsException()
    {
        // Arrange
        SetupHttpError(_mockHttpClient, HttpStatusCode.NotFound);
        
        // Act & Assert
        await Assert.ThrowsExceptionAsync<PoeApiException>(
            () => _wrapper.GetCurrencyDataAsync("InvalidLeague"));
    }
}
```

### Message Parser Tests

```csharp
[TestClass]
public class MessageParserTests
{
    private MessageParser _parser;
    
    [TestInitialize]
    public void Setup()
    {
        _parser = new MessageParser();
    }
    
    [TestMethod]
    public void ParseTradeMessage_StandardFormat_ExtractsCorrectData()
    {
        // Arrange
        var message = "Hi, I would like to buy your Tabula Rasa listed for 10 chaos in Standard";
        
        // Act
        var result = _parser.ParseTradeMessage(message);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Tabula Rasa", result.ItemName);
        Assert.AreEqual(10, result.Price);
        Assert.AreEqual("chaos", result.Currency);
        Assert.AreEqual("Standard", result.League);
    }
    
    [TestMethod]
    public void ParseTradeMessage_VariantFormats_HandlesAllPatterns()
    {
        var testCases = new[]
        {
            ("wtb your Kaom's Heart 5ex", "Kaom's Heart", 5m, "ex"),
            ("@player Hi, want to trade?", null, 0m, null),
            ("buying mirror 500 div", "mirror", 500m, "div")
        };
        
        foreach (var (input, expectedItem, expectedPrice, expectedCurrency) in testCases)
        {
            var result = _parser.ParseTradeMessage(input);
            
            if (expectedItem != null)
            {
                Assert.AreEqual(expectedItem, result.ItemName, $"Failed for: {input}");
                Assert.AreEqual(expectedPrice, result.Price, $"Failed for: {input}");
                Assert.AreEqual(expectedCurrency, result.Currency, $"Failed for: {input}");
            }
            else
            {
                Assert.IsNull(result, $"Should be null for: {input}");
            }
        }
    }
}
```

### Cache Behavior Tests

```csharp
[TestClass]
public class CurrencyPriceCacheTests
{
    private CurrencyPriceCache _cache;
    private Mock<IPriceFetcher> _mockPriceFetcher;
    
    [TestInitialize]
    public void Setup()
    {
        _mockPriceFetcher = new Mock<IPriceFetcher>();
        _cache = new CurrencyPriceCache(_mockPriceFetcher.Object);
    }
    
    [TestMethod]
    public async Task GetPrice_FirstCall_FetchesFromSource()
    {
        // Arrange
        _mockPriceFetcher.Setup(x => x.GetPriceAsync("chaos", "Standard"))
                        .ReturnsAsync(100m);
        
        // Act
        var price = await _cache.GetPriceAsync("chaos", "Standard");
        
        // Assert
        Assert.AreEqual(100m, price);
        _mockPriceFetcher.Verify(x => x.GetPriceAsync("chaos", "Standard"), Times.Once);
    }
    
    [TestMethod]
    public async Task GetPrice_SecondCall_ReturnsFromCache()
    {
        // Arrange
        _mockPriceFetcher.Setup(x => x.GetPriceAsync("chaos", "Standard"))
                        .ReturnsAsync(100m);
        
        // Act
        await _cache.GetPriceAsync("chaos", "Standard");
        var price = await _cache.GetPriceAsync("chaos", "Standard");
        
        // Assert
        Assert.AreEqual(100m, price);
        _mockPriceFetcher.Verify(x => x.GetPriceAsync("chaos", "Standard"), Times.Once);
    }
    
    [TestMethod]
    public async Task GetPrice_ExpiredCache_RefetchesData()
    {
        // Arrange
        var shortCache = new CurrencyPriceCache(_mockPriceFetcher.Object, TimeSpan.FromMilliseconds(50));
        _mockPriceFetcher.SetupSequence(x => x.GetPriceAsync("chaos", "Standard"))
                        .ReturnsAsync(100m)
                        .ReturnsAsync(150m);
        
        // Act
        var price1 = await shortCache.GetPriceAsync("chaos", "Standard");
        await Task.Delay(100); // Wait for cache expiration
        var price2 = await shortCache.GetPriceAsync("chaos", "Standard");
        
        // Assert
        Assert.AreEqual(100m, price1);
        Assert.AreEqual(150m, price2);
        _mockPriceFetcher.Verify(x => x.GetPriceAsync("chaos", "Standard"), Times.Exactly(2));
    }
}
```

### Exception Handling Tests

```csharp
[TestClass]
public class ExceptionsTests
{
    [TestMethod]
    public void PoeApiException_WithMessage_SetsPropertiesCorrectly()
    {
        // Arrange
        var message = "API request failed";
        var statusCode = HttpStatusCode.TooManyRequests;
        
        // Act
        var exception = new PoeApiException(message, statusCode);
        
        // Assert
        Assert.AreEqual(message, exception.Message);
        Assert.AreEqual(statusCode, exception.StatusCode);
        Assert.IsTrue(exception.IsRateLimited);
    }
    
    [TestMethod]
    public void PriceNotFoundException_WithCurrency_FormatsMessageCorrectly()
    {
        // Arrange
        var currency = "mirror";
        var league = "Standard";
        
        // Act
        var exception = new PriceNotFoundException(currency, league);
        
        // Assert
        Assert.AreEqual(currency, exception.Currency);
        Assert.AreEqual(league, exception.League);
        Assert.IsTrue(exception.Message.Contains(currency));
        Assert.IsTrue(exception.Message.Contains(league));
    }
}
```

## 🔧 Test Utilities

### Mock HTTP Responses

```csharp
public static class TestHttpHelper
{
    public static void SetupHttpResponse(Mock<HttpClient> mockClient, string jsonResponse)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };
        
        mockClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                 .ReturnsAsync(response);
    }
    
    public static void SetupHttpError(Mock<HttpClient> mockClient, HttpStatusCode statusCode)
    {
        var response = new HttpResponseMessage(statusCode);
        
        mockClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                 .ReturnsAsync(response);
    }
    
    public static string LoadTestData(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"PoeLib.Tests.TestData.{fileName}";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        
        return reader.ReadToEnd();
    }
}
```

### Test Data Builders

```csharp
public class CurrencyDataBuilder
{
    private readonly CurrencyData _data = new();
    
    public CurrencyDataBuilder WithName(string name)
    {
        _data.CurrencyTypeName = name;
        return this;
    }
    
    public CurrencyDataBuilder WithValue(decimal value)
    {
        _data.ChaosEquivalent = value;
        return this;
    }
    
    public CurrencyDataBuilder WithConfidence(int confidence)
    {
        _data.Confidence = confidence;
        return this;
    }
    
    public CurrencyData Build() => _data;
}

public class TradeMessageBuilder
{
    private readonly TradeMessage _message = new();
    
    public TradeMessageBuilder WithItem(string itemName)
    {
        _message.ItemName = itemName;
        return this;
    }
    
    public TradeMessageBuilder WithPrice(decimal price, string currency)
    {
        _message.Price = price;
        _message.Currency = currency;
        return this;
    }
    
    public TradeMessage Build() => _message;
}
```

## 📊 Performance Tests

### Load Testing

```csharp
[TestClass]
public class PerformanceTests
{
    [TestMethod]
    public async Task CurrencyCache_ConcurrentAccess_HandlesLoad()
    {
        // Arrange
        var cache = new CurrencyPriceCache(new MockPriceFetcher());
        var tasks = new List<Task<decimal?>>();
        
        // Act
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(cache.GetPriceAsync("chaos", "Standard"));
        }
        
        var results = await Task.WhenAll(tasks);
        
        // Assert
        Assert.IsTrue(results.All(r => r.HasValue));
        Assert.IsTrue(results.All(r => r.Value > 0));
    }
    
    [TestMethod]
    public void MessageParser_ParseLargeVolume_CompletesInTime()
    {
        // Arrange
        var parser = new MessageParser();
        var messages = GenerateTestMessages(10000);
        var stopwatch = Stopwatch.StartNew();
        
        // Act
        var results = messages.Select(m => parser.ParseTradeMessage(m)).ToList();
        stopwatch.Stop();
        
        // Assert
        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000, "Parsing took too long");
        Assert.IsTrue(results.Count(r => r != null) > 0, "No messages were parsed");
    }
}
```

## 🏃‍♂️ Running Tests

### Command Line

```bash
# Run all tests
dotnet test PoeLib.Tests/

# Run specific test categories
dotnet test --filter "Category=PriceFetcher"
dotnet test --filter "Category=MessageParser"
dotnet test --filter "Category=Cache"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run performance tests
dotnet test --filter "Category=Performance"
```

### Visual Studio

```
Test Explorer → Run All Tests
Test Explorer → Group By → Category
Test Explorer → Filter → Search for specific tests
```

## 📁 Test Data

### Sample API Responses

```json
// TestData/poe-ninja-currency-response.json
{
  "lines": [
    {
      "currencyTypeName": "Chaos Orb",
      "chaosEquivalent": 1.0,
      "confidence": 10
    },
    {
      "currencyTypeName": "Divine Orb", 
      "chaosEquivalent": 200.0,
      "confidence": 10
    }
  ]
}
```

### Test Message Patterns

```csharp
public static class TestMessagePatterns
{
    public static readonly string[] ValidTradeMessages = 
    {
        "Hi, I would like to buy your Tabula Rasa listed for 10 chaos in Standard",
        "@player wtb your mirror 500 divine",
        "buying kaom's heart for 15 ex in hardcore",
        "Hi, I'd like to buy your Atziri's Disfavour listed for 25 divine in Standard"
    };
    
    public static readonly string[] InvalidMessages =
    {
        "Hello there!",
        "What's the price?",
        "Still available?",
        "Random chat message"
    };
}
```

## ⚙️ Configuration

### Test Settings

```json
{
  "TestSettings": {
    "UseRealHttpClient": false,
    "ApiTimeout": 5000,
    "CacheTestDuration": 100,
    "MaxConcurrentTests": 10
  },
  "MockSettings": {
    "SimulateNetworkDelay": true,
    "DefaultResponseDelay": 50,
    "FailureRate": 0.1
  }
}
```

## 🔍 Test Analysis

### Coverage Reports

```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report"
```

### Test Metrics

- **Code Coverage**: Target 90%+ for critical paths
- **Performance**: All tests complete under 10 seconds
- **Reliability**: 100% pass rate on clean builds
- **Maintainability**: Tests update automatically with code changes

## 🤝 Contributing Tests

When adding new tests:

1. **Follow Naming Convention**: `MethodName_Scenario_ExpectedResult`
2. **Use AAA Pattern**: Arrange, Act, Assert clearly separated
3. **Mock External Dependencies**: Don't call real APIs in unit tests
4. **Test Edge Cases**: Include null, empty, and invalid inputs
5. **Add Performance Tests**: For critical performance paths

### Test Template

```csharp
[TestMethod]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var input = "test input";
    var expected = "expected result";
    var service = new ServiceUnderTest();
    
    // Act
    var actual = service.ProcessInput(input);
    
    // Assert
    Assert.AreEqual(expected, actual);
}
```

## 🔗 Related Projects

- [PoeLib](../PoeLib/) - The library being tested
- [PoeTrade.Contracts.Tests](../PoeTrade.Contracts.Tests/) - Contract model tests
- [PoeTradeMonitor.GUI.Tests](../PoeTradeMonitor.GUI.Tests/) - GUI component tests
- [PoeTradeMonitor.Service.Tests](../PoeTradeMonitor.Service.Tests/) - Service layer tests