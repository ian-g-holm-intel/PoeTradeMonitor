# PoeTrade.Contracts

Comprehensive JSON contracts and data models for Path of Exile 1 and Path of Exile 2 trade API integration. This library provides strongly-typed models for all POE trade API requests and responses, ensuring type safety and cross-game compatibility.

## 🎯 Purpose

PoeTrade.Contracts serves as the central contract library for:
- **Trade API Integration**: Complete models for POE1 and POE2 trade APIs
- **Cross-Game Compatibility**: Unified interfaces supporting both games
- **JSON Serialization**: Custom converters for complex API responses
- **Protocol Buffer Support**: Efficient binary serialization for gRPC
- **Type Safety**: Strongly-typed models preventing runtime errors
- **Comprehensive Testing**: 245+ tests ensuring reliability

## Project Structure

### Core Contracts

#### Response Models
- **`TradeFetchResponse.cs`** - Root response from trade search API
- **`TradeSearchResult.cs`** - Individual trade listing result
- **`TradeItem.cs`** - Item details (mods, sockets, properties)
- **`TradeListing.cs`** - Listing metadata (price, account, stash)
- **`TradeAccount.cs`** - Account and online status information

#### Request Models
- **`TradeSearchRequest.cs`** - Root request for trade searches
- **`TradeQuery.cs`** - Search query parameters
- **`TradeFilters.cs`** - Search filter categories
- **`TypeFilters.cs`** - Item type and rarity filters
- **`WeaponFilters.cs`** - PoE1 weapon-specific filters
- **`ArmourFilters.cs`** - PoE1 armor-specific filters
- **`EquipmentFilters.cs`** - PoE2 equipment filters
- **`MiscFilters.cs`** - Miscellaneous item filters

### Extensions

#### Protobuf Conversion
- **`ProtobufConversionExtensions.cs`** - Response record ↔ protobuf conversion
- **`ProtobufRequestConversionExtensions.cs`** - Request record ↔ protobuf conversion
- **`ProtobufFilterConversionExtensions.cs`** - Filter-specific conversions

### Protobuf Definitions

Located in the `Proto/` directory:
- **`trade_response.proto`** - Response message definitions
- **`trade_request.proto`** - Request message definitions  
- **`request_filters.proto`** - Filter message definitions
- **`complex_filters.proto`** - Advanced filter definitions

## Game Version Support

### Path of Exile 1
- Legacy filter types (weapon_filters, armour_filters, socket_filters)
- League-specific content (heist_filters, sanctum_filters, ultimatum_filters)
- Traditional currency system

### Path of Exile 2
- Modern equipment system (equipment_filters)
- New mechanics (rune_sockets, spirit, reload_time)
- Updated item categories and rarities

### Cross-Compatibility
All records use nullable properties to accommodate differences between game versions. The same contract types can deserialize both PoE1 and PoE2 responses.

## Usage Examples

### Deserializing a Trade Response
```csharp
using System.Text.Json;
using PoeTradeMonitor.Contracts;

var jsonResponse = await httpClient.GetStringAsync("https://www.pathofexile.com/api/trade/search/...");
var response = JsonSerializer.Deserialize<TradeFetchResponse>(jsonResponse);

foreach (var result in response.Result)
{
    Console.WriteLine($"Item: {result.Item.Name} - Price: {result.Listing.Price?.Amount} {result.Listing.Price?.Currency}");
}
```

### Creating a Trade Search Request
```csharp
var request = new TradeSearchRequest
{
    Query = new TradeQuery
    {
        Status = new OptionFilter { Option = "online" },
        Filters = new TradeFilters
        {
            TypeFilters = new TypeFilters
            {
                Filters = new TypeFilterOptions
                {
                    Category = new OptionFilter { Option = "weapon" },
                    Rarity = new OptionFilter { Option = "unique" }
                }
            },
            TradeFilters = new TradeFilters
            {
                Filters = new TradeFilterOptions
                {
                    Price = new PriceFilter 
                    { 
                        Option = "divine", 
                        Min = 1, 
                        Max = 100 
                    }
                }
            }
        }
    },
    Sort = new SortOptions { Price = "asc" }
};
```

### Protobuf Conversion
```csharp
using PoeTradeMonitor.Contracts.Extensions;

// Convert to protobuf for gRPC
var protoRequest = request.ToProtobuf();

// Convert back to record
var originalRequest = protoRequest.ToRecord();
```

## JSON Examples

The project includes comprehensive JSON examples in `JSON Examples/`:
- `Responses/PoE1/` - Real PoE1 API responses
- `Responses/PoE2/` - Real PoE2 API responses  
- `Requests/PoE1/` - PoE1 search request examples
- `Requests/PoE2/` - PoE2 search request examples

## Dependencies

- **System.Text.Json** - JSON serialization
- **Google.Protobuf** - Protobuf serialization
- **Grpc.Net.Client** - gRPC client support

## Testing

Comprehensive test coverage is provided in the `PoeTradeMonitor.Contracts.Tests` project:
- JSON deserialization tests for both game versions
- Protobuf conversion round-trip tests
- Cross-compatibility validation
- Filter-specific conversion tests

## Building

```bash
dotnet build PoeTradeMonitor.Contracts
```

Protobuf files are automatically compiled during build using the Grpc.Tools package.