# PoeTradeMonitor.GUI

Modern WPF desktop application providing a comprehensive user interface for Path of Exile trading operations, featuring real-time monitoring, advanced search capabilities, and seamless integration with automated trading services.

## Overview

PoeTradeMonitor.GUI is the primary user interface for the PoeTradeMonitor ecosystem, built using Windows Presentation Foundation (WPF) and the Model-View-ViewModel (MVVM) pattern. It provides traders with powerful tools for monitoring POE markets, managing trade requests, and coordinating with the automated trading service.

## Features

### Trading Interface
- **Real-time Trade Monitoring**: Live display of incoming trade requests and market opportunities
- **Advanced Item Search**: Sophisticated search interface with customizable criteria
- **Live Search Integration**: Real-time monitoring of POE trade API for matching items
- **Price Validation**: Multi-source price checking with visual indicators
- **Trade Request Management**: Queue and prioritize trade requests with scheduling

### User Experience
- **Modern WPF Interface**: Clean, responsive design optimized for trading workflows
- **Audio Notifications**: Customizable sound alerts for trade opportunities
- **Real-time Updates**: Live data synchronization with minimal latency

### Data Management
- **Stash Integration**: Direct integration with POE stash API for inventory management
- **Currency Tracking**: Real-time currency price monitoring and conversion
- **Search Persistence**: Save and manage custom search criteria
- **Statistics Tracking**: Comprehensive trading statistics and analytics
- **Cache Management**: Intelligent caching for improved performance

### Service Integration
- **gRPC Communication**: High-performance communication with background service
- **Service Discovery**: Automatic detection and connection to local/remote services
- **Callback Handling**: Real-time notifications from automated trading service
- **Configuration Sync**: Seamless settings synchronization across components

## Architecture

### MVVM Pattern
The application follows the Model-View-ViewModel pattern for maintainable and testable code:

#### ViewModels (`ViewModels/`)
- **MainWindowViewModel** - Primary application interface and coordination
- **NewSearchDialogWindowViewModel** - Search criteria configuration
- **SetServerIPWindowViewModel** - Service connection management

#### Views (`Views/`)
- **MainWindow.xaml** - Primary application window
- **NewSearchDialogWindow.xaml** - Search configuration dialog
- **SetServerIPWindow.xaml** - Service configuration window

#### Models (`Models/`)
- **SearchGuiItem** - Search result data representation
- **StashGuiItem** - Stash inventory item representation

### Service Layer

#### Communication Clients (`Clients/`)
- **TradeBotClient** - Interface to automated trading service
- **PartyManagerClient** - Party coordination and management
- **PoeHttpClient** - POE API communication with authentication

#### Core Services (`Services/`)
- **TradeRequestScheduler** - Queue and prioritize trade requests
- **LiveSearchResultProcessor** - Process real-time search results
- **PoePriceChecker** - Multi-source price validation
- **StashDataUpdater** - Synchronize stash inventory data
- **BrowserService** - External browser integration
- **CallbackService** - Handle service callbacks and notifications

#### Data Management
- **ItemPriceCache** - Cache item pricing data
- **CurrencyCache** - Cache currency exchange rates
- **LiveSearchItemCache** - Cache live search results

### Item Search System

#### Search Components (`ItemSearch/`)
- **PoeItemSearch** - Core search functionality
- **PoeItemLiveSearch** - Real-time search monitoring
- **CustomSearchManager** - Manage saved search criteria
- **SearchCriteriaMatcher** - Match items against search criteria
- **PoeItemSearchRequestCache** - Cache search requests for performance

#### Data Retrievers (`DataRetrievers/`)
- **StashItemsRetriever** - Fetch stash inventory data
- **StashCurrencyRetriever** - Fetch currency holdings
- **CurrencyPriceRetriever** - Fetch current market prices

## Configuration

### Application Settings
- **appsettings.json** - Production configuration
- **appsettings.Development.json** - Development-specific settings
- **settings.json** - User preferences and trading configuration

### Settings Management (`Settings/`)
- **PoeSettings** - Core application settings
- **BrowserSettings** - Browser integration configuration
- **IgnoredItem** - Item filtering and exclusion rules
- **SettingsManager** - Live settings management with hot-reload

### Environment Variables
Required for full functionality:
- **POE_ACCOUNT_NAME** - Path of Exile account name for API access
- **PUSHOVER_API_TOKEN** - Push notification API token
- **PUSHOVER_USER_KEY** - Push notification user key
- **BRAVE_BROWSER_PATH** - Custom browser path (optional)

## Dependencies

### UI Framework
- **WPF** - Windows Presentation Foundation for native Windows UI
- **CommunityToolkit.Mvvm** - Modern MVVM implementation with source generators

### Communication
- **Grpc.AspNetCore** - gRPC server hosting for callbacks
- **Grpc.Net.Client** - gRPC client for service communication
- **System.Text.Json** - High-performance JSON serialization

### External Integration
- **PoeAuthenticator** - POE account authentication and session management
- **PoeLib** - Core trading functionality and data models
- **PoeTrade.Contracts** - Shared contract definitions

### Utilities
- **Nito.AsyncEx** - Advanced async/await patterns
- **Serilog.AspNetCore** - Structured logging framework
- **TestableIO.System.IO.Abstractions.Wrappers** - Testable file system operations

## Usage

### Starting the Application
```bash
dotnet run --project PoeTradeMonitor.GUI
```

### Initial Setup
1. **Configure POE Account**: Set POE_ACCOUNT_NAME environment variable
2. **Authentication**: Log in to Path of Exile through the application
3. **Service Connection**: Ensure PoeTradeMonitor.Service is running
4. **Notification Setup**: Configure Pushover credentials for alerts

### Core Workflows

#### Setting Up Searches
1. Click "New Search" to open search configuration
2. Define item criteria, price ranges, and quality requirements
3. Enable live search for real-time monitoring
4. Save search for future use

#### Managing Trades
1. Monitor incoming trade requests in the main interface
2. Use price validation to verify trade values
3. Queue high-priority trades for automated execution
4. Track trade statistics and success rates

#### Stash Management
1. View current stash contents with real-time synchronization
2. Monitor currency holdings and conversion rates
3. Track valuable items and their market values
4. Set up automated inventory alerts

## Development

### Building
```bash
dotnet build PoeTradeMonitor.GUI/PoeTradeMonitor.GUI.csproj
```

### Testing
```bash
dotnet test PoeTradeMonitor.GUI.Tests/PoeTradeMonitor.GUI.Tests.csproj
```

### Debugging
- Full Visual Studio debugging support with WPF designer
- XAML Hot Reload for rapid UI development
- Structured logging with Serilog for runtime diagnostics

## Integration Points

### Service Communication
- **gRPC Server**: Hosts callback service on port 5001
- **gRPC Client**: Connects to trading service on port 5002
- **HTTP/2**: Modern protocol for optimal performance

### POE API Integration
- **Trade API**: Real-time access to POE trading data
- **Stash API**: Direct inventory access and management
- **Authentication**: Secure session management with automatic renewal
- **Rate Limiting**: Intelligent API usage with respect for rate limits

### External Services
- **PoeNinja**: Real-time market price data
- **PoeWatch**: Alternative price validation source
- **Pushover**: Push notifications for mobile devices
- **Browser Integration**: Launch external trade pages and resources

## Performance Optimization

### Caching Strategy
- **Background Updates**: Non-blocking cache refresh operations

### UI Responsiveness
- **Async Operations**: All network and disk operations are asynchronous
- **Background Processing**: Heavy operations run on background threads

### Resource Management
- **Memory Optimization**: Efficient object lifecycle management
- **Connection Pooling**: Reuse HTTP connections for better performance

## Security & Privacy

### API Security
- **Rate Limiting**: Respect external API limits to prevent bans
- **Error Handling**: Graceful degradation when services are unavailable
- **Audit Logging**: Comprehensive logging for security analysis