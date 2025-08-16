# PoeHudWrapper

Integration layer for Path of Exile overlay functionality, providing a bridge between the PoeTradeMonitor ecosystem and the ExileAPI framework for in-game overlays and automation.

## 🎯 Purpose

PoeHudWrapper serves as the integration layer for:
- **Game Overlay Integration**: Connect with Path of Exile overlay systems
- **Memory Reading Bridge**: Abstract ExileAPI memory reading functionality
- **Plugin Communication**: Interface with ExileAPI plugins and extensions
- **Real-time Game State**: Provide live game state information to other components
- **Cross-Process Communication**: Bridge between different application processes

## 🏗️ Architecture

### Core Components

```csharp
public class PoeHudWrapper
{
    private readonly ExileApiConnection _exileApi;
    private readonly MemoryReader _memoryReader;
    private readonly OverlayManager _overlayManager;
    
    public async Task<bool> InitializeAsync()
    {
        try
        {
            await _exileApi.ConnectAsync();
            _memoryReader.Initialize(_exileApi.GameController);
            _overlayManager.Setup(_exileApi.Graphics);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize PoeHudWrapper");
            return false;
        }
    }
    
    public GameState GetCurrentGameState()
    {
        return new GameState
        {
            Player = GetPlayerInformation(),
            Inventory = GetInventoryState(),
            Area = GetCurrentArea(),
            UI = GetUIState(),
            Timestamp = DateTime.UtcNow
        };
    }
}
```

### Memory Object Abstraction

```csharp
public class ModValue
{
    public string Name { get; set; }
    public int Value { get; set; }
    public ModValueType Type { get; set; }
    public bool IsImplicit { get; set; }
    
    public static ModValue FromExileApiMod(ExileApi.Pocos.Mod mod)
    {
        return new ModValue
        {
            Name = mod.Name,
            Value = mod.Value1,
            Type = MapModType(mod.ModRecord.ModType),
            IsImplicit = mod.ModRecord.IsImplicit
        };
    }
}
```

## 🎮 Game State Integration

### Player Information

```csharp
public class PlayerInformation
{
    public string Name { get; set; }
    public int Level { get; set; }
    public string Class { get; set; }
    public Vector2 Position { get; set; }
    public PlayerStats Stats { get; set; }
    public bool IsAlive { get; set; }
    public string CurrentArea { get; set; }
    
    public static PlayerInformation FromGameController(GameController gc)
    {
        var player = gc.Player;
        return new PlayerInformation
        {
            Name = player.GetComponent<Player>()?.PlayerName ?? "Unknown",
            Level = player.GetComponent<Player>()?.Level ?? 0,
            Class = player.GetComponent<Player>()?.ClassName ?? "Unknown",
            Position = player.GridPos,
            Stats = PlayerStats.FromStatsComponent(player.GetComponent<Stats>()),
            IsAlive = player.IsAlive,
            CurrentArea = gc.Area.CurrentArea.Name
        };
    }
}
```

### Inventory Management

```csharp
public class InventoryState
{
    public List<ItemInformation> Items { get; set; } = new();
    public int UsedSlots { get; set; }
    public int TotalSlots { get; set; }
    public CurrencyInfo Currency { get; set; }
    
    public static InventoryState FromInventoryHolder(InventoryHolder inventory)
    {
        var items = new List<ItemInformation>();
        
        foreach (var item in inventory.Inventory.InventorySlotItems)
        {
            if (item.Item != null)
            {
                items.Add(ItemInformation.FromEntity(item.Item));
            }
        }
        
        return new InventoryState
        {
            Items = items,
            UsedSlots = items.Count,
            TotalSlots = inventory.Inventory.TotalBoxes,
            Currency = CurrencyInfo.FromInventory(inventory)
        };
    }
}
```

### UI State Monitoring

```csharp
public class UIState
{
    public bool IsChatOpen { get; set; }
    public bool IsInventoryOpen { get; set; }
    public bool IsTradeWindowOpen { get; set; }
    public bool IsInHideout { get; set; }
    public bool IsInTown { get; set; }
    
    public static UIState FromIngameState(IngameState ingameState)
    {
        return new UIState
        {
            IsChatOpen = ingameState.IngameUi.ChatPanel.IsVisible,
            IsInventoryOpen = ingameState.IngameUi.InventoryPanel.IsVisible,
            IsTradeWindowOpen = ingameState.IngameUi.TradeWindow.IsVisible,
            IsInHideout = ingameState.Data.LocalPlayer.Area.Name.Contains("Hideout"),
            IsInTown = ingameState.Data.LocalPlayer.Area.IsTown
        };
    }
}
```

## 🔄 Real-time Updates

### Event-Driven Architecture

```csharp
public class GameStateMonitor
{
    public event EventHandler<PlayerMoved> OnPlayerMoved;
    public event EventHandler<InventoryChanged> OnInventoryChanged;
    public event EventHandler<AreaChanged> OnAreaChanged;
    public event EventHandler<TradeWindowStateChanged> OnTradeWindowStateChanged;
    
    private readonly Timer _updateTimer;
    private GameState _previousState;
    
    public GameStateMonitor()
    {
        _updateTimer = new Timer(CheckForChanges, null, 
            TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
    }
    
    private void CheckForChanges(object state)
    {
        var currentState = _wrapper.GetCurrentGameState();
        
        if (_previousState != null)
        {
            CompareStates(_previousState, currentState);
        }
        
        _previousState = currentState;
    }
    
    private void CompareStates(GameState previous, GameState current)
    {
        // Player position changes
        if (Vector2.Distance(previous.Player.Position, current.Player.Position) > 0.5f)
        {
            OnPlayerMoved?.Invoke(this, new PlayerMoved
            {
                OldPosition = previous.Player.Position,
                NewPosition = current.Player.Position
            });
        }
        
        // Inventory changes
        if (previous.Inventory.Items.Count != current.Inventory.Items.Count)
        {
            OnInventoryChanged?.Invoke(this, new InventoryChanged
            {
                AddedItems = current.Inventory.Items.Except(previous.Inventory.Items).ToList(),
                RemovedItems = previous.Inventory.Items.Except(current.Inventory.Items).ToList()
            });
        }
        
        // Area changes
        if (previous.Area != current.Area)
        {
            OnAreaChanged?.Invoke(this, new AreaChanged
            {
                OldArea = previous.Area,
                NewArea = current.Area
            });
        }
    }
}
```

### Overlay Integration

```csharp
public class OverlayManager
{
    private readonly Graphics _graphics;
    private readonly List<IOverlayComponent> _components = new();
    
    public void RegisterComponent(IOverlayComponent component)
    {
        _components.Add(component);
        component.Initialize(_graphics);
    }
    
    public void Render()
    {
        foreach (var component in _components.Where(c => c.IsVisible))
        {
            try
            {
                component.Render(_graphics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to render component {component.GetType().Name}");
            }
        }
    }
}

public interface IOverlayComponent
{
    bool IsVisible { get; }
    void Initialize(Graphics graphics);
    void Render(Graphics graphics);
    void Update(GameState gameState);
}
```

## 🔧 Extension Methods

### Game Object Extensions

```csharp
public static class ExtensionMethods
{
    public static bool IsValidGameObject(this Entity entity)
    {
        return entity != null && 
               entity.IsValid && 
               entity.Address != IntPtr.Zero &&
               !entity.GetComponent<ObjectMagicProperties>()?.Rarity.HasFlag(ItemRarity.Deleted) == true;
    }
    
    public static string GetItemName(this Entity item)
    {
        var baseComponent = item.GetComponent<Base>();
        var modsComponent = item.GetComponent<Mods>();
        
        if (modsComponent?.ItemRarity == ItemRarity.Unique)
        {
            return modsComponent.UniqueName;
        }
        
        return baseComponent?.Name ?? "Unknown Item";
    }
    
    public static List<ModValue> GetAllMods(this Entity item)
    {
        var modsComponent = item.GetComponent<Mods>();
        if (modsComponent == null)
            return new List<ModValue>();
            
        var allMods = new List<ModValue>();
        
        // Implicit mods
        allMods.AddRange(modsComponent.ImplicitMods.Select(ModValue.FromExileApiMod));
        
        // Explicit mods
        allMods.AddRange(modsComponent.ExplicitMods.Select(ModValue.FromExileApiMod));
        
        return allMods;
    }
}
```

## 🚀 Usage Examples

### Basic Game State Monitoring

```csharp
// Initialize wrapper
var wrapper = new PoeHudWrapper();
await wrapper.InitializeAsync();

// Set up monitoring
var monitor = new GameStateMonitor(wrapper);
monitor.OnPlayerMoved += (sender, e) => 
    Console.WriteLine($"Player moved to {e.NewPosition}");
monitor.OnInventoryChanged += (sender, e) => 
    Console.WriteLine($"Inventory changed: +{e.AddedItems.Count}, -{e.RemovedItems.Count}");

// Get current state
var gameState = wrapper.GetCurrentGameState();
Console.WriteLine($"Player: {gameState.Player.Name} (Level {gameState.Player.Level})");
Console.WriteLine($"Area: {gameState.Area}");
Console.WriteLine($"Items in inventory: {gameState.Inventory.Items.Count}");
```

### Integration with Trade Bot

```csharp
public class TradeBotIntegration
{
    private readonly PoeHudWrapper _wrapper;
    private readonly TradeBotService _tradeBot;
    
    public async Task<bool> PerformTradeAsync(TradeRequest request)
    {
        // Check if we're in a safe trading state
        var gameState = _wrapper.GetCurrentGameState();
        if (!gameState.UI.IsInHideout && !gameState.UI.IsInTown)
        {
            return false;
        }
        
        // Monitor for trade window
        var monitor = new GameStateMonitor(_wrapper);
        var tradeCompleted = false;
        
        monitor.OnTradeWindowStateChanged += (sender, e) =>
        {
            if (e.IsOpen)
            {
                // Validate trade contents
                var tradeItems = GetTradeWindowItems();
                if (ValidateTradeItems(tradeItems, request))
                {
                    AcceptTrade();
                    tradeCompleted = true;
                }
            }
        };
        
        // Initiate trade
        SendTradeWhisper(request);
        
        // Wait for completion or timeout
        var timeout = TimeSpan.FromMinutes(2);
        var startTime = DateTime.UtcNow;
        
        while (!tradeCompleted && DateTime.UtcNow - startTime < timeout)
        {
            await Task.Delay(100);
        }
        
        return tradeCompleted;
    }
}
```

## 📁 Project Structure

```
PoeHudWrapper/
├── Bootstrapper.cs          # Dependency injection setup
├── PoeHudWrapper.cs         # Main wrapper class
├── ExtensionMethods.cs      # Game object extensions
├── Usings.cs               # Global using statements
├── MemoryObjects/          # Memory object abstractions
│   └── ModValue.cs
├── Events/                 # Event argument classes
│   ├── PlayerMoved.cs
│   ├── InventoryChanged.cs
│   └── AreaChanged.cs
├── Monitoring/             # Real-time monitoring
│   ├── GameStateMonitor.cs
│   └── OverlayManager.cs
└── Integration/            # External service integration
    ├── TradeBotIntegration.cs
    └── OverlayComponents.cs
```

## 🧪 Testing

```bash
# Run wrapper tests
dotnet test PoeHudWrapper.Tests/

# Test game state monitoring
dotnet test --filter "GameStateMonitorTests"

# Test memory object conversion
dotnet test --filter "MemoryObjectTests"
```

## ⚙️ Configuration

### Wrapper Settings

```json
{
  "PoeHudWrapper": {
    "UpdateInterval": 100,
    "EnableOverlay": true,
    "EnableMemoryReading": true,
    "MaxReconnectAttempts": 5,
    "ReconnectDelay": 5000
  },
  "Monitoring": {
    "PlayerMovementThreshold": 0.5,
    "InventoryCheckInterval": 200,
    "UIStateCheckInterval": 100
  },
  "Overlay": {
    "EnableTradeNotifications": true,
    "EnableInventoryHighlight": false,
    "EnablePositionDisplay": false
  }
}
```

## 🔍 Troubleshooting

### Common Issues

**ExileAPI Connection Failed**
- Ensure ExileAPI is running and accessible
- Check process permissions and injection status
- Verify game version compatibility

**Memory Reading Errors**
- Update memory offsets for current game version
- Check for anti-cheat interference
- Ensure proper process privileges

**Overlay Not Visible**
- Check overlay rendering permissions
- Verify graphics initialization
- Ensure game is in windowed/borderless mode

### Debugging Tools

```csharp
public class WrapperDiagnostics
{
    public async Task<HealthCheckResult> PerformHealthCheckAsync()
    {
        var result = new HealthCheckResult();
        
        // Check ExileAPI connection
        result.ExileApiConnected = await TestExileApiConnection();
        
        // Check memory reading
        result.MemoryReadingWorking = await TestMemoryReading();
        
        // Check overlay rendering
        result.OverlayRendering = await TestOverlayRendering();
        
        // Check game state updates
        result.GameStateUpdating = await TestGameStateUpdates();
        
        return result;
    }
    
    public void LogCurrentGameState()
    {
        var state = _wrapper.GetCurrentGameState();
        _logger.LogInformation($"Game State: Player={state.Player.Name}, Area={state.Area}, " +
                             $"Inventory={state.Inventory.Items.Count} items");
    }
}
```

## 🤝 Contributing

When contributing to PoeHudWrapper:

1. **Maintain Compatibility**: Ensure changes work with current ExileAPI version
2. **Handle Errors Gracefully**: Memory reading can fail, implement proper error handling
3. **Performance First**: Minimize performance impact on game
4. **Test Thoroughly**: Verify functionality across different game states
5. **Document APIs**: Clear documentation for public interfaces

## 🔗 Related Projects

- [PrivateExileApi](../PrivateExileApi/) - Core ExileAPI implementation
- [PoeTradeMonitor.Service](../PoeTradeMonitor.Service/) - Trading automation service
- [OffsetFinder](../OffsetFinder/) - Memory offset discovery utility
- [GameOffsets](../PrivateExileApi/GameOffsets/) - Game memory structure definitions