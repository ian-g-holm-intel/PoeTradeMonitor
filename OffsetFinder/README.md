# OffsetFinder

Memory offset discovery utility for Path of Exile game integration. This tool automatically finds and validates memory addresses used for reading game state and enabling automation features.

## 🎯 Purpose

OffsetFinder provides essential functionality for game integration:
- **Memory Offset Discovery**: Automatically locate game data structures in memory
- **Pattern Scanning**: Use byte patterns to find dynamic addresses
- **Offset Validation**: Verify discovered offsets work correctly
- **Version Compatibility**: Handle different game versions and updates
- **Integration Support**: Generate offset definitions for other tools

## 🔍 Core Features

### Pattern-Based Discovery
- **Signature Scanning**: Find memory patterns using AOB (Array of Bytes) signatures
- **Relative Address Calculation**: Calculate offsets from discovered addresses
- **Multi-Pattern Support**: Use multiple patterns for robust discovery
- **Version-Specific Patterns**: Different patterns for different game versions

### Memory Structure Analysis
- **Game Object Discovery**: Locate player, inventory, and world objects
- **UI Element Mapping**: Find game interface elements and windows
- **Data Structure Validation**: Verify discovered structures contain expected data
- **Pointer Chain Resolution**: Follow multi-level pointer chains

## 🏗️ Architecture

### Core Components

```csharp
public class OffsetFinder
{
    private readonly Process _gameProcess;
    private readonly Dictionary<string, MemoryPattern> _patterns;
    
    public async Task<Dictionary<string, IntPtr>> DiscoverOffsetsAsync()
    {
        var results = new Dictionary<string, IntPtr>();
        
        foreach (var pattern in _patterns)
        {
            var address = await ScanForPattern(pattern.Value);
            if (address != IntPtr.Zero)
            {
                results[pattern.Key] = CalculateOffset(address, pattern.Value.Offset);
                _logger.LogInformation($"Found {pattern.Key} at 0x{address:X}");
            }
            else
            {
                _logger.LogWarning($"Pattern not found: {pattern.Key}");
            }
        }
        
        return results;
    }
}
```

### Pattern Definition

```csharp
public class MemoryPattern
{
    public string Name { get; set; }
    public byte[] Pattern { get; set; }
    public string Mask { get; set; }
    public int Offset { get; set; }
    public bool IsRelativeAddress { get; set; }
    public int RelativeOffset { get; set; }
    
    public static MemoryPattern Create(string name, string pattern, int offset = 0)
    {
        var bytes = ParsePatternString(pattern);
        var mask = GenerateMask(pattern);
        
        return new MemoryPattern
        {
            Name = name,
            Pattern = bytes,
            Mask = mask,
            Offset = offset
        };
    }
}
```

## 🎮 Game Integration

### Player Data Discovery

```csharp
public class PlayerDataOffsets
{
    public static readonly MemoryPattern PlayerBasePattern = MemoryPattern.Create(
        "PlayerBase",
        "48 8B 05 ? ? ? ? 48 8B 50 ? 48 8B 49 ? 48 85 C9",
        3);
        
    public static readonly MemoryPattern PlayerStatsPattern = MemoryPattern.Create(
        "PlayerStats", 
        "48 8B 87 ? ? ? ? 48 85 C0 74 ? 48 8B 40 ?",
        3);
        
    public static readonly MemoryPattern PlayerPositionPattern = MemoryPattern.Create(
        "PlayerPosition",
        "F3 0F 10 86 ? ? ? ? F3 0F 11 45 ? F3 0F 10 86",
        4);
}
```

### Inventory System

```csharp
public class InventoryOffsets
{
    public static readonly MemoryPattern InventoryBasePattern = MemoryPattern.Create(
        "InventoryBase",
        "48 8B 0D ? ? ? ? 48 85 C9 0F 84 ? ? ? ? 48 8B 81",
        3);
        
    public static readonly MemoryPattern InventoryGridPattern = MemoryPattern.Create(
        "InventoryGrid",
        "48 8B 87 ? ? ? ? 48 8B CF 48 89 45 ? 48 85 C0",
        3);
        
    public static readonly MemoryPattern ItemListPattern = MemoryPattern.Create(
        "ItemList",
        "48 8B 8F ? ? ? ? 48 85 C9 74 ? 48 8B 01 FF 50",
        3);
}
```

### UI Element Discovery

```csharp
public class UIOffsets
{
    public static readonly MemoryPattern IngameUIPattern = MemoryPattern.Create(
        "IngameUI",
        "48 8B 0D ? ? ? ? 48 85 C9 74 ? 48 8B 89 ? ? ? ?",
        3);
        
    public static readonly MemoryPattern ChatWindowPattern = MemoryPattern.Create(
        "ChatWindow",
        "48 8B 91 ? ? ? ? 48 85 D2 74 ? 48 8B 82",
        3);
        
    public static readonly MemoryPattern TradeWindowPattern = MemoryPattern.Create(
        "TradeWindow",
        "48 8B 89 ? ? ? ? 48 85 C9 0F 84 ? ? ? ? 80 B9",
        3);
}
```

## 🔧 Pattern Scanning

### Memory Scanner Implementation

```csharp
public class MemoryScanner
{
    private readonly Process _process;
    private readonly ProcessModule _module;
    
    public async Task<IntPtr> ScanForPatternAsync(MemoryPattern pattern)
    {
        var moduleBase = _module.BaseAddress;
        var moduleSize = _module.ModuleMemorySize;
        
        var buffer = new byte[moduleSize];
        if (!ReadProcessMemory(_process.Handle, moduleBase, buffer, moduleSize, out _))
        {
            throw new Win32Exception("Failed to read process memory");
        }
        
        return await Task.Run(() => FindPattern(buffer, pattern));
    }
    
    private IntPtr FindPattern(byte[] buffer, MemoryPattern pattern)
    {
        for (int i = 0; i < buffer.Length - pattern.Pattern.Length; i++)
        {
            if (MatchesPattern(buffer, i, pattern))
            {
                var address = _module.BaseAddress + i;
                
                if (pattern.IsRelativeAddress)
                {
                    var relativeAddress = BitConverter.ToInt32(buffer, i + pattern.RelativeOffset);
                    return address + pattern.RelativeOffset + 4 + relativeAddress;
                }
                
                return address + pattern.Offset;
            }
        }
        
        return IntPtr.Zero;
    }
    
    private bool MatchesPattern(byte[] buffer, int offset, MemoryPattern pattern)
    {
        for (int i = 0; i < pattern.Pattern.Length; i++)
        {
            if (pattern.Mask[i] == 'x' && buffer[offset + i] != pattern.Pattern[i])
            {
                return false;
            }
        }
        return true;
    }
}
```

### Pattern Validation

```csharp
public class OffsetValidator
{
    public async Task<bool> ValidateOffsetAsync(string offsetName, IntPtr address)
    {
        try
        {
            switch (offsetName)
            {
                case "PlayerBase":
                    return await ValidatePlayerBase(address);
                case "InventoryBase":
                    return await ValidateInventoryBase(address);
                case "IngameUI":
                    return await ValidateIngameUI(address);
                default:
                    return await ValidateGenericOffset(address);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to validate offset {offsetName}");
            return false;
        }
    }
    
    private async Task<bool> ValidatePlayerBase(IntPtr address)
    {
        // Read player object and verify structure
        var playerObject = ReadMemory<PlayerObject>(address);
        
        // Basic validation checks
        return playerObject.Id > 0 && 
               playerObject.Level > 0 && 
               playerObject.Level <= 100 &&
               !string.IsNullOrEmpty(playerObject.Name);
    }
}
```

## 📊 Offset Management

### Offset Storage

```csharp
public class OffsetStorage
{
    private readonly string _offsetFilePath;
    
    public void SaveOffsets(Dictionary<string, IntPtr> offsets, string gameVersion)
    {
        var offsetData = new OffsetData
        {
            GameVersion = gameVersion,
            Timestamp = DateTime.UtcNow,
            Offsets = offsets.ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value.ToInt64())
        };
        
        var json = JsonSerializer.Serialize(offsetData, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        File.WriteAllText(_offsetFilePath, json);
        _logger.LogInformation($"Saved {offsets.Count} offsets for version {gameVersion}");
    }
    
    public Dictionary<string, IntPtr> LoadOffsets(string gameVersion)
    {
        if (!File.Exists(_offsetFilePath))
            return new Dictionary<string, IntPtr>();
            
        var json = File.ReadAllText(_offsetFilePath);
        var offsetData = JsonSerializer.Deserialize<OffsetData>(json);
        
        if (offsetData.GameVersion != gameVersion)
        {
            _logger.LogWarning($"Offset version mismatch: stored {offsetData.GameVersion}, current {gameVersion}");
            return new Dictionary<string, IntPtr>();
        }
        
        return offsetData.Offsets.ToDictionary(
            kvp => kvp.Key,
            kvp => new IntPtr(kvp.Value));
    }
}
```

### Version Detection

```csharp
public class GameVersionDetector
{
    public string GetGameVersion(Process gameProcess)
    {
        try
        {
            var mainModule = gameProcess.MainModule;
            var versionInfo = FileVersionInfo.GetVersionInfo(mainModule.FileName);
            
            return $"{versionInfo.ProductMajorPart}.{versionInfo.ProductMinorPart}." +
                   $"{versionInfo.ProductBuildPart}.{versionInfo.ProductPrivatePart}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to detect game version");
            return "Unknown";
        }
    }
    
    public bool IsVersionSupported(string version)
    {
        var supportedVersions = new[]
        {
            "3.23.0", "3.23.1", "3.23.2",
            "3.24.0", "3.24.1", "3.24.2",
            "4.0.0"  // PoE2
        };
        
        return supportedVersions.Contains(version) || 
               supportedVersions.Any(v => version.StartsWith(v.Split('.')[0] + "."));
    }
}
```

## 🚀 Usage Examples

### Basic Offset Discovery

```csharp
// Find offsets for current game version
var finder = new OffsetFinder();
var gameProcess = Process.GetProcessesByName("PathOfExile_x64").FirstOrDefault();

if (gameProcess != null)
{
    var offsets = await finder.DiscoverOffsetsAsync(gameProcess);
    
    foreach (var offset in offsets)
    {
        Console.WriteLine($"{offset.Key}: 0x{offset.Value:X}");
    }
    
    // Save for future use
    var storage = new OffsetStorage();
    var version = new GameVersionDetector().GetGameVersion(gameProcess);
    storage.SaveOffsets(offsets, version);
}
```

### Automated Offset Update

```csharp
public class OffsetUpdateService
{
    public async Task<bool> UpdateOffsetsIfNeededAsync()
    {
        var gameProcess = GetGameProcess();
        if (gameProcess == null)
            return false;
            
        var currentVersion = _versionDetector.GetGameVersion(gameProcess);
        var storedOffsets = _storage.LoadOffsets(currentVersion);
        
        if (storedOffsets.Count == 0)
        {
            _logger.LogInformation($"Discovering offsets for new version: {currentVersion}");
            var newOffsets = await _finder.DiscoverOffsetsAsync(gameProcess);
            
            if (await ValidateAllOffsets(newOffsets))
            {
                _storage.SaveOffsets(newOffsets, currentVersion);
                return true;
            }
        }
        
        return storedOffsets.Count > 0;
    }
}
```

## 📁 Project Structure

```
OffsetFinder/
├── Program.cs              # Console application entry point
├── OffsetFinder.csproj     # Project configuration
├── Patterns/               # Memory pattern definitions
│   ├── PlayerPatterns.cs
│   ├── InventoryPatterns.cs
│   ├── UIPatterns.cs
│   └── GameStatePatterns.cs
├── Scanners/              # Memory scanning implementations
│   ├── MemoryScanner.cs
│   ├── PatternMatcher.cs
│   └── AddressCalculator.cs
├── Validation/            # Offset validation logic
│   ├── OffsetValidator.cs
│   └── StructureValidator.cs
└── Storage/               # Offset persistence
    ├── OffsetStorage.cs
    └── VersionManager.cs
```

## 🧪 Testing

```bash
# Run offset discovery tests
dotnet test OffsetFinder.Tests/

# Test specific pattern categories
dotnet test --filter "PlayerPatternTests"
dotnet test --filter "InventoryPatternTests"
```

## ⚙️ Configuration

### Pattern Configuration

```json
{
  "Patterns": {
    "PlayerBase": {
      "Pattern": "48 8B 05 ? ? ? ? 48 8B 50 ? 48 8B 49",
      "Offset": 3,
      "IsRelative": true,
      "Priority": "High"
    },
    "InventoryBase": {
      "Pattern": "48 8B 0D ? ? ? ? 48 85 C9 0F 84",
      "Offset": 3,
      "IsRelative": true,
      "Priority": "High"
    }
  },
  "Validation": {
    "EnableStructureValidation": true,
    "MaxValidationAttempts": 3,
    "ValidationTimeout": 5000
  },
  "Storage": {
    "OffsetCacheEnabled": true,
    "CacheExpirationHours": 24,
    "BackupPreviousVersions": true
  }
}
```

## 🔍 Troubleshooting

### Common Issues

**Game Process Not Found**
- Ensure Path of Exile is running
- Check process name (PathOfExile vs PathOfExile_x64)
- Verify administrator privileges

**Patterns Not Found**
- Game version may have changed
- Update pattern definitions
- Check memory protection settings

**Invalid Offsets**
- Validate discovered addresses
- Check for ASLR (Address Space Layout Randomization)
- Update patterns for current game version

### Debugging Tools

```csharp
public class OffsetDebugger
{
    public void DumpMemoryRegion(IntPtr address, int size)
    {
        var buffer = new byte[size];
        if (ReadProcessMemory(_process.Handle, address, buffer, size, out _))
        {
            var hex = BitConverter.ToString(buffer).Replace("-", " ");
            _logger.LogDebug($"Memory at 0x{address:X}: {hex}");
        }
    }
    
    public void AnalyzePatternMatches(MemoryPattern pattern)
    {
        var matches = FindAllPatternMatches(pattern);
        _logger.LogInformation($"Pattern '{pattern.Name}' found {matches.Count} matches");
        
        foreach (var match in matches)
        {
            _logger.LogDebug($"  Match at 0x{match:X}");
        }
    }
}
```

## 🤝 Contributing

When contributing to OffsetFinder:

1. **Test Thoroughly**: Verify patterns work across game versions
2. **Document Patterns**: Explain what each pattern is looking for
3. **Add Validation**: Include validation logic for new offsets
4. **Handle Errors**: Graceful handling of pattern discovery failures
5. **Version Support**: Ensure backwards compatibility

## 🔗 Related Projects

- [PrivateExileApi](../PrivateExileApi/) - Game integration API that uses these offsets
- [PoeHudWrapper](../PoeHudWrapper/) - Game overlay functionality
- [PoeTradeMonitor.Service](../PoeTradeMonitor.Service/) - Automated trading service
- [GameOffsets](../PrivateExileApi/GameOffsets/) - Shared offset definitions