# PoeTradeMonitor.Service

Background service application that handles automated Path of Exile trading operations, chat monitoring, and trade execution using advanced automation techniques.

## Overview

PoeTradeMonitor.Service is a high-performance background service that runs continuously to monitor Path of Exile trades, execute automated trading operations, and provide real-time game integration. The service operates autonomously while the game is running and communicates with the GUI application via gRPC.

## Features

### Automated Trading
- **Trade Bot Integration**: Fully automated trade execution using state machines
- **Smart Trade Execution**: Context-aware trading with safety checks and validation
- **Party Management**: Automatic party invitation and management
- **Input Simulation**: Advanced input automation using AutoIt and InputSimulator
- **Trade State Tracking**: Comprehensive state machine for trade workflow management

### Real-time Monitoring
- **Chat Monitoring**: Real-time Path of Exile chat analysis and message parsing
- **Game Process Integration**: Automatic lifecycle management tied to game process
- **Memory Integration**: Direct game memory access through PoeHudWrapper

### Price Validation & Analysis
- **Multi-source Price Validation**: Cross-reference prices from multiple APIs
- **Real-time Price Fetching**: Live currency and item price data
- **Price Caching**: Optimized caching for improved performance
- **Smart Pricing Algorithms**: Advanced price validation and recommendation logic

### Communication Services
- **gRPC Server**: High-performance service communication on port 5002
- **Callback Services**: Real-time event notifications to GUI clients
- **HTTP/2 Protocol**: Modern communication protocol for optimal performance
- **Service Discovery**: Automatic endpoint configuration and management

## Architecture

### Core Services

#### Trading Components (`Services/`)
- **TradeBotService** - Main gRPC service for trade automation
- **TradeExecutorService** - Handles trade execution workflow
- **TradeBotStateMachine** - State management for complex trade operations
- **PartyManagerService** - Manages party invitations and member coordination

#### Communication (`Clients/`)
- **CallbackClient** - Handles callbacks to GUI application
- gRPC client configuration for reliable inter-service communication

#### Game Integration
- **TradeBot** - Core automation engine using AutoIt and Windows APIs
- **TradeCommands** - High-level trade command abstractions
- **PriceValidator** - Real-time price validation and analysis

### Dependencies & Integration

#### Game Automation
- **AutoItX3** - Windows automation for game interaction
- **InputSimulatorStandard** - Advanced input simulation
- **PoeHudWrapper** - Game overlay and memory access integration
- **Win32 APIs** - Direct Windows system integration

#### External Services
- **Serilog** - Structured logging with file and console outputs
- **gRPC & HTTP/2** - High-performance service communication
- **RateLimiter** - API rate limiting and throttling
- **SixLabors.ImageSharp** - Image processing for game analysis

## Configuration

### Application Settings
- **appsettings.json** - Production configuration
- **appsettings.Development.json** - Development-specific settings
- **GuiAddress** - Configurable GUI service address (default: 127.0.0.1:5001)

### Environment Variables
Required for secure operation:
- **POE_ACCOUNT_NAME** - Path of Exile account name
- **PUSHOVER_API_TOKEN** - Push notification API token
- **PUSHOVER_USER_KEY** - Push notification user key

### Logging Configuration
- **Production**: Minimal logging to reduce noise
- **Development**: Verbose logging with source context
- **File Logging**: Rotating log files in `%ProgramData%/PoeTradeMonitor/`
- **Console Logging**: Real-time information level output

## Usage

### Starting the Service
```bash
dotnet run --project PoeTradeMonitor.Service
```

### Prerequisites
- Path of Exile must be running (service auto-exits when game closes)
- Required environment variables must be configured
- GUI application should be running for full functionality

### Service Endpoints
- **gRPC Server**: `http://localhost:5002`
- **TradeBotService**: Main trading automation service
- **PartyManagerService**: Party management and coordination
- **Health Check**: Root endpoint provides service status

### Process Management
The service automatically:
- Detects Path of Exile process startup/shutdown
- Positions console window for optimal visibility
- Manages lifecycle tied to game process
- Handles graceful shutdown when game exits

## Development

### Building
```bash
dotnet build PoeTradeMonitor.Service/PoeTradeMonitor.Service.csproj
```

### Testing
```bash
dotnet test PoeTradeMonitor.Service.Tests/PoeTradeMonitor.Service.Tests.csproj
```

### Debugging
- Launch with Visual Studio or VS Code for full debugging support
- Console window auto-positions for multi-monitor development setups
- Structured logging provides detailed execution context

## Security & Safety

### Input Validation
- All trade requests validated before execution
- Price validation with configurable thresholds
- Safe automation with built-in delays and checks

### Error Handling
- Comprehensive exception handling and logging
- Graceful degradation when external services are unavailable
- Automatic recovery mechanisms for transient failures

### Data Protection
- No sensitive data stored in logs or configuration files
- Environment variable-based security configuration
- Rate limiting to prevent API abuse

## Integration

### GUI Communication
- Real-time status updates via gRPC callbacks
- Trade request processing and response handling
- Configuration synchronization

### Game Integration
- Non-invasive memory reading through ExileAPI
- Safe input automation with game state awareness
- Overlay integration for visual feedback

### External APIs
- PoeNinja and PoeWatch price data integration
- Pushover notification delivery
- Configurable proxy support for rate limiting

## Monitoring & Observability

### Logging
- Structured JSON logging with Serilog
- Configurable log levels per component
- File rotation and retention policies

### Health Monitoring
- Process lifecycle tracking
- Service endpoint health checks
- Performance metrics collection

### Notifications
- Real-time trade alerts via Pushover
- Error notifications for critical failures
- Configurable notification thresholds