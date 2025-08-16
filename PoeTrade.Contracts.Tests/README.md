# PoeTradeMonitor.Contracts.Tests

This project contains comprehensive tests for the PoeTradeMonitor.Contracts project, ensuring the reliability and correctness of all contract types, JSON serialization, and protobuf conversion functionality for both Path of Exile 1 and Path of Exile 2.

## Test Organization

The test suite is organized by game version and functionality to provide clear separation and maintainability:

### Response Tests (Legacy)
- **`PoE1DeserializationTests.cs`** - Path of Exile 1 response deserialization tests
- **`PoE2DeserializationTests.cs`** - Path of Exile 2 response deserialization tests
- **`CrossCompatibilityTests.cs`** - Cross-compatibility tests between PoE1 and PoE2 responses
- **`ResponseProtobufConversionTests.cs`** - Response protobuf conversion and round-trip tests

### Request Tests (Current)
- **`PoE1RequestDeserializationTests.cs`** - Path of Exile 1 request deserialization tests
- **`PoE2RequestDeserializationTests.cs`** - Path of Exile 2 request deserialization tests
- **`RequestCrossCompatibilityTests.cs`** - Cross-compatibility tests between PoE1 and PoE2 requests
- **`PoE1RequestProtobufConversionTests.cs`** - PoE1 request protobuf conversion tests
- **`PoE2RequestProtobufConversionTests.cs`** - PoE2 request protobuf conversion tests
- **`RequestProtobufFilterConversionTests.cs`** - Common filter type protobuf conversion tests

### Shared Infrastructure
- **`BaseDeserializationTests.cs`** - Base class with common test utilities and helper methods

## Test Data Structure

Test data is organized in the `TestData` directory and automatically copied during build:

```
TestData/
├── Responses/
│   ├── PoE1/
│   │   ├── Weapon.json, Accessory.json, BodyArmour.json
│   │   ├── AbyssJewel.json, BaseJewel.json, ClusterJewel.json
│   │   ├── Map.json, MapFragment.json, DivinationCard.json
│   │   ├── HeistBlueprint.json, HeistContract.json, HeistEquipment.json
│   │   └── ... (20+ response examples)
│   └── PoE2/
│       ├── Weapon.json, Accessory.json, BodyArmour.json
│       ├── Jewel.json, Talisman.json, Waystone.json
│       ├── Gems.json, Flask.json, Misc.json
│       └── ... (11+ response examples)
└── Requests/
    ├── PoE1/
    │   ├── Full_Yes.json, Min_No.json, Max_Any.json
    └── PoE2/
        ├── Full_Yes.json, Min_No.json, Max_Any.json
```

## Test Categories

### JSON Deserialization Tests
- **Purpose**: Verify that JSON responses/requests from the Path of Exile trade API can be correctly deserialized into C# record types
- **Coverage**: All game versions (PoE1 and PoE2), various item types and search scenarios
- **Validation**: Basic structure, required properties, game-specific differences

### Protobuf Conversion Tests
- **Purpose**: Ensure bidirectional conversion between C# records and protobuf messages works correctly
- **Coverage**: Round-trip conversion testing (Record → Protobuf → Record)
- **Validation**: Data integrity, null handling, nested object preservation

### Cross-Compatibility Tests
- **Purpose**: Verify that the same contract types can handle both PoE1 and PoE2 data
- **Coverage**: Game version differences, nullable properties, version-specific filters
- **Validation**: Proper handling of game-specific features while maintaining compatibility

### Filter-Specific Tests
- **Purpose**: Test individual filter type conversions (RangeFilter, OptionFilter, PriceFilter, etc.)
- **Coverage**: All filter types with various data scenarios (null values, partial data, complete data)
- **Validation**: Accurate conversion of filter criteria and edge cases

## Key Test Scenarios

### PoE1 Specific Tests
- Legacy filter types (weapon_filters, armour_filters, socket_filters)
- League-specific content (heist_filters, sanctum_filters, ultimatum_filters)
- Traditional currency systems and pricing
- PoE1 item types: AbyssJewel, BaseJewel, ClusterJewel, DivinationCard, ExpeditionLogbook, HeistBlueprint, HeistContract, HeistEquipment, MapFragment, SkillGem, SupportGem, Tattoo

### PoE2 Specific Tests
- Modern equipment system (equipment_filters)
- New mechanics (rune_sockets, spirit, reload_time)
- Updated item categories and rarities
- Enhanced quality and level filtering
- PoE2 item types: Jewel, Talisman, Waystone, enhanced Gems system

### Common Test Patterns
- **Data Integrity**: Ensure all properties are correctly mapped
- **Null Safety**: Verify proper handling of optional/nullable properties
- **Type Safety**: Confirm strong typing throughout the conversion process
- **Edge Cases**: Extended property variations (object `{}` vs array `[]`), magnitude values (strings vs numbers)

## Special Handling

The tests handle various edge cases in the JSON data:
- **Extended property**: Can be either an object `{}` or empty array `[]`
- **Magnitude values**: Can be strings `"10"` or numbers `10`
- **Optional properties**: Many properties are nullable to handle missing fields
- **Version differences**: PoE2 has additional fields like `realm`, `grantedSkills`, `runeMods`, etc.
- **Filter differences**: PoE1 uses separate weapon/armor filters, PoE2 uses unified equipment filters

## Running Tests

### All Tests
```bash
dotnet test PoeTradeMonitor.Contracts.Tests
```

### Specific Test Categories
```bash
# Response tests only
dotnet test --filter "ClassName~DeserializationTests OR ClassName~CrossCompatibilityTests OR ClassName~ResponseProtobuf"

# Request tests only  
dotnet test --filter "ClassName~RequestDeserializationTests OR ClassName~RequestProtobuf OR ClassName~RequestCrossCompatibility"

# PoE1 tests only
dotnet test --filter "ClassName~PoE1"

# PoE2 tests only
dotnet test --filter "ClassName~PoE2"

# Protobuf conversion tests only
dotnet test --filter "ClassName~Protobuf"
```

## Test Statistics

- **Total Tests**: 72
- **Response Tests**: 44 (covering 20+ PoE1 response types and 11+ PoE2 response types)
- **Request Tests**: 28 (covering deserialization and protobuf conversion for both game versions)
- **Coverage**: 100% of public contract types and conversion methods

All tests should pass, verifying complete compatibility with both PoE1 and PoE2 trade JSON formats and robust protobuf conversion functionality.