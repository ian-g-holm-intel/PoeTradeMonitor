# PoeTradeMonitor

> **⚠️ Important Notice:** This project depends on a private repository that is not included in this public repository. The application will not run without access to these private dependencies. This repository is provided for reference and educational purposes only.

A comprehensive C# .NET 9.0 Windows application suite for Path of Exile trading automation, featuring real-time monitoring, intelligent price validation, and advanced automation capabilities.

## Overview

PoeTradeMonitor is a sophisticated trading solution designed for Path of Exile players who want to optimize their trading experience through automation and advanced analytics. The system combines a modern WPF desktop interface with a powerful background service to provide seamless trading operations while maintaining game immersion.

## 🚀 Key Features

### Automated Trading
- **Intelligent Trade Bot**: Fully automated trade execution with safety checks
- **Smart Scheduling**: Queue and prioritize trades based on profitability
- **Party Management**: Automatic party invitations and coordination
- **Input Automation**: Advanced game interaction using AutoIt integration
- **State Management**: Robust state machines for complex trading workflows

### Real-time Monitoring
- **Live Trade Alerts**: Instant notifications for profitable opportunities
- **Multi-source Price Validation**: Cross-reference prices from PoeNinja and PoeWatch
- **Chat Integration**: Real-time Path of Exile chat monitoring and parsing
- **Market Analysis**: Continuous monitoring of currency and item markets

### Advanced Search & Analytics
- **Custom Search Criteria**: Sophisticated item filtering and matching
- **Live Search Integration**: Real-time POE trade API monitoring
- **Price Trend Analysis**: Historical price tracking and prediction

### Modern Interface
- **Responsive WPF UI**: Clean, intuitive desktop application
- **Real-time Updates**: Live data synchronization with minimal latency
- **Customizable Notifications**: Audio and push notification alerts

## 🏗️ Architecture

### Core Components

#### [PoeTradeMonitor.GUI](PoeTradeMonitor.GUI/)
Modern WPF desktop application providing the primary user interface for trade monitoring and configuration.

**Key Features:**
- Real-time trade monitoring dashboard
- Advanced item search interface
- Stash integration and inventory management
- Service configuration and status monitoring
- Push notification setup and management

#### [PoeTradeMonitor.Service](PoeTradeMonitor.Service/)
Background service handling automated trading operations and game integration.

**Key Features:**
- Automated trade execution engine
- Real-time chat monitoring and parsing
- Game process integration and overlay support
- gRPC service endpoints for GUI communication
- Advanced input automation with safety checks

#### [PoeLib](PoeLib/)
Core shared library containing essential functionality for POE API interactions and data processing.

**Key Features:**
- POE API integration and authentication
- Multi-source price fetching (PoeNinja, PoeWatch)
- gRPC communication infrastructure
- Message parsing and validation
- Caching and performance optimization

### Supporting Libraries

- **PoeTrade.Contracts** - JSON contracts and data models for POE1/POE2 items
- **PoeAuthenticator** - Secure authentication and session management
- **PoeHudWrapper** - Game overlay integration and memory access
- **PoeCrafter** - Item crafting simulation and optimization tools
- **OffsetFinder** - Memory offset discovery for game integration

## 🛠️ Technology Stack

### Core Technologies
- **.NET 9.0** - Modern cross-platform framework targeting Windows
- **C#** - Primary programming language with nullable reference types
- **WPF** - Windows Presentation Foundation for native desktop UI
- **gRPC** - High-performance inter-service communication
- **Protocol Buffers** - Efficient serialization for service communication

### Key Dependencies
- **AutoIt** - Windows automation for game interaction
- **Serilog** - Structured logging framework
- **CommunityToolkit.Mvvm** - Modern MVVM pattern implementation
- **System.Text.Json** - High-performance JSON processing
- **InputSimulator** - Advanced input simulation capabilities

### External Integrations
- **Path of Exile APIs** - Trade, stash, and account data access
- **PoeNinja API** - Real-time currency and item pricing
- **PoeWatch API** - Alternative price validation source
- **Pushover** - Mobile push notification delivery

## ⚙️ Installation & Setup

### Prerequisites
- **Windows 10/11** (x64) - Required for game integration
- **.NET 9.0 Runtime** - Download from Microsoft
- **Path of Exile** - Game must be installed and running
- **POE Account** - Valid Path of Exile account for API access

### Environment Variables
Configure the following environment variables for full functionality:

```bash
# Required
POE_ACCOUNT_NAME=your_poe_account_name
PUSHOVER_API_TOKEN=your_pushover_api_token
PUSHOVER_USER_KEY=your_pushover_user_key

# Optional
BRAVE_BROWSER_PATH=C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe
```

### Quick Start

1. **Clone the Repository**
   ```bash
   git clone https://github.com/your-repo/PoeTradeMonitor.git
   cd PoeTradeMonitor
   ```

2. **Build the Solution**
   ```bash
   dotnet build PoeTradeMonitor.sln
   ```

3. **Run Tests**
   ```bash
   dotnet test
   ```

4. **Start the Applications**
   ```bash
   # Start the background service first
   dotnet run --project PoeTradeMonitor.Service
   
   # Start the GUI application
   dotnet run --project PoeTradeMonitor.GUI
   ```

## 🎮 Usage

### Initial Configuration
1. **Launch Applications**: Start both Service and GUI applications
2. **Account Setup**: Configure POE account credentials in the GUI
3. **Service Connection**: Verify GUI connects to the background service
4. **Notification Setup**: Configure Pushover for mobile alerts
5. **Game Integration**: Ensure Path of Exile is running for full functionality

### Basic Workflow
1. **Create Searches**: Define item criteria and price ranges
2. **Monitor Markets**: Watch for profitable trading opportunities
3. **Queue Trades**: Add promising trades to the automation queue
4. **Execute Trades**: Let the service handle automated trade execution
5. **Track Performance**: Monitor statistics and adjust strategies

### Advanced Features
- **Custom Filters**: Create sophisticated item matching rules
- **Profit Thresholds**: Set minimum profit margins for automation
- **Risk Management**: Configure safety limits and validation rules
- **Multi-account Support**: Manage multiple POE accounts and characters

## 🔧 Development

### Building from Source
```bash
# Clean and build entire solution
dotnet clean && dotnet build PoeTradeMonitor.sln

# Build specific projects
dotnet build PoeLib/PoeLib.csproj
dotnet build PoeTradeMonitor.GUI/PoeTradeMonitor.GUI.csproj
dotnet build PoeTradeMonitor.Service/PoeTradeMonitor.Service.csproj
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test projects
dotnet test PoeLib.Tests/PoeLib.Tests.csproj
dotnet test PoeTradeMonitor.GUI.Tests/PoeTradeMonitor.GUI.Tests.csproj
dotnet test PoeTradeMonitor.Service.Tests/PoeTradeMonitor.Service.Tests.csproj
```

### Code Quality
- **Nullable Reference Types**: Enabled across all projects
- **Treat Warnings as Errors**: Enforced code quality standards
- **EditorConfig**: Consistent code style and formatting
- **Static Analysis**: Automated code quality checks

## 📊 Performance & Scalability

### Optimization Features
- **Multi-level Caching**: Memory, disk, and network cache layers
- **Connection Pooling**: Efficient HTTP connection reuse
- **Rate Limiting**: Intelligent API usage within service limits
- **Background Processing**: Non-blocking operations for UI responsiveness

### Monitoring & Observability
- **Structured Logging**: Comprehensive logging with Serilog
- **Performance Metrics**: Built-in performance tracking
- **Health Checks**: Service availability monitoring
- **Error Tracking**: Detailed error reporting and analysis

## 🔒 Security & Privacy

### Data Protection
- **No Data Collection**: No user data transmitted to third parties
- **Credential Management**: Secure storage of authentication tokens

### Safety Features
- **Input Validation**: Comprehensive validation of all user inputs
- **Rate Limiting**: Protection against API abuse and bans
- **Error Handling**: Graceful degradation during service outages
- **Audit Logging**: Complete audit trail for security analysis

## 🙏 Acknowledgments

- **Path of Exile** by Grinding Gear Games
- **PoeNinja** for comprehensive market data
- **PoeWatch** for alternative price sources
- **ExileAPI** community for game integration insights
- **AutoIt** team for Windows automation capabilities
