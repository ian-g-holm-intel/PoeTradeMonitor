# PoeLib

Core shared library for Path of Exile trading functionality, providing essential services for POE API interactions, price fetching, and data processing.

## Overview

PoeLib serves as the foundation library for the PoeTradeMonitor ecosystem, containing shared functionality used across both the GUI and Service applications. It provides a comprehensive set of tools for interacting with Path of Exile's trading systems.

## Features

### Price Fetching
- **PoeNinja Integration**: Real-time currency and item pricing from poe.ninja
- **PoeWatch Integration**: Alternative price data source from poe.watch  
- **Price Caching**: In-memory caching for improved performance
- **Multi-source Price Validation**: Compare prices across different data providers

### Communication Services
- **gRPC Services**: Type-safe inter-service communication using Protocol Buffers
- **Message Parsing**: Parse and validate trade-related messages

### Game Integration
- **Chat Monitoring**: Watch Path of Exile chat for trade messages
- **Win32 API Integration**: Native Windows API interactions for game integration
- **Message Caching**: Efficient storage and retrieval of chat messages

### Notification Services
- **Pushover Integration**: Push notifications for trade alerts and status updates
- **Configurable Notifications**: Customizable notification settings and triggers

## Architecture

### Core Components

#### Price Fetching (`PriceFetchers/`)
- `PoeNinjaWrapper` - Fetches pricing data from poe.ninja API
- `PoeWatchWrapper` - Fetches pricing data from poe.watch API  
- `PriceFetcherWrapper` - Aggregates multiple price sources
- `CurrencyPriceCache` - In-memory caching for currency exchange rates

#### Data Models (`JSON/`)
- `PoeNinja/` - Data contracts for poe.ninja API responses
- `PoeWatch/` - Data contracts for poe.watch API responses
- Strongly-typed models for currency data, item data, and statistics

#### Communication (`gRPC/`, `Proto/`)
- Protocol Buffer definitions for service contracts
- gRPC service implementations for inter-component communication
- Unix domain socket connection factory for local communication

#### Tools and Utilities (`Tools/`)
- `PoeChatWatcher` - Monitor Path of Exile chat for trade messages
- `ChatMessageCache` - Efficient message storage and retrieval
- `PushoverNotificationClient` - Push notification delivery
- `Win32` - Native Windows API wrapper functions

#### Common Components (`Common/`)
- `BaseTypes` - Fundamental data types and structures
- `Constants` - Application-wide constants and configuration values
- `DataClasses` - Core data transfer objects
- `Enums` - Enumeration definitions for various game concepts
- `Exceptions` - Custom exception types for error handling
- `Interfaces` - Service and component interface definitions

#### Extensions (`Extensions/`)
- `ExtensionMethods` - Utility extension methods for common operations
- `MessageConverters` - Convert between different message formats

## Dependencies

### Core Dependencies
- **.NET 9.0** - Target framework with nullable reference types
- **Google.Protobuf** - Protocol buffer serialization
- **Grpc.Net.Client** - gRPC client library
- **Grpc.Core.Api** - gRPC core API

### External Services
- **Microsoft.Extensions.Http** - HTTP client factory for API calls
- **Microsoft.Extensions.Caching.Memory** - In-memory caching support
- **Microsoft.Extensions.Hosting.Abstractions** - Hosting and dependency injection
- **PushoverNET.Standard** - Pushover notification service
- **Nito.AsyncEx** - Advanced async/await utilities

### Project References
- **PoeTrade.Contracts** - Shared contract definitions for POE1/POE2 items

## Usage

### Dependency Injection Setup

```csharp
using PoeLib;

// In your service registration
services.AddPoeLib();
```

The `AddPoeLib()` extension method registers all core services:
- `ICurrencyPriceCache` - Currency price caching
- `IChatMessageCache` - Chat message storage
- `INotificationClient` - Push notification delivery
- `IPriceFetcher` - Price data fetching (multiple implementations)
- `IPriceFetcherWrapper` - Aggregated price fetching
- `IMessageParser` - Message parsing and validation
- `IPoeChatWatcher` - Chat monitoring functionality

### Environment Variables

The following environment variables are required for full functionality:

- **POE_ACCOUNT_NAME** - Path of Exile account name for API requests
- **PUSHOVER_API_TOKEN** - Pushover API token for push notifications  
- **PUSHOVER_USER_KEY** - Pushover user/group key for notifications

## Development

### Building
```bash
dotnet build PoeLib/PoeLib.csproj
```

### Testing  
```bash
dotnet test PoeLib.Tests/PoeLib.Tests.csproj
```

### Code Style
This project follows the repository's .editorconfig conventions:
- File-scoped namespaces
- Nullable reference types enabled
- TreatWarningsAsErrors enabled for code quality

## Integration

PoeLib is designed to be used by:
- **PoeTradeMonitor.GUI** - Desktop application interface
- **PoeTradeMonitor.Service** - Background trading service
- **Other POE-related applications** requiring core trading functionality

The library provides a clean abstraction layer between game-specific logic and application implementations, making it easy to extend and maintain trading functionality across different components of the system.