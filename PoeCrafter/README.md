# PoeCrafter

Advanced item crafting simulation and optimization tools for Path of Exile. This console application provides crafting calculations, optimal strategies, and item analysis for various crafting scenarios.

## 🎯 Purpose

PoeCrafter helps Path of Exile players optimize their crafting strategies by:
- **Crafting Simulation**: Simulate various crafting methods and outcomes
- **Cost Analysis**: Calculate expected costs for achieving desired modifiers
- **Strategy Optimization**: Find the most efficient crafting paths
- **Item Analysis**: Analyze existing items for potential improvements
- **Mod Group Analysis**: Understand modifier interactions and conflicts

## 🔧 Core Features

### Crafting Simulation
- **Alt-Regal Crafting**: Optimize alteration and augmentation usage
- **Chaos Spamming**: Calculate optimal chaos orb strategies
- **Fossil Crafting**: Simulate fossil combinations and outcomes
- **Essence Crafting**: Model essence-based crafting approaches
- **Beast Crafting**: Include beast crafting possibilities

### Item Categories

#### Weapons
- **Physical Weapons**: One-handed and two-handed melee weapons
- **Elemental Weapons**: Wands, sceptres, and elemental damage optimization
- **Bows**: Physical and elemental bow crafting
- **Special Weapons**: Death's Oath, Femurs of the Saints optimization

#### Armor
- **Energy Shield Armor**: ES-based chest pieces and helmets
- **Life-based Armor**: High life and resistance optimization
- **Hybrid Builds**: Life/ES combinations
- **Movement Speed Items**: Boots with optimal speed rolls

#### Accessories
- **Rings**: Damage, life, and resistance optimization
- **Amulets**: Specialized builds (mana, focused amulets)
- **Belts**: Heavy belt and chain belt optimization

### Advanced Features

#### Jewel Crafting
- **Viridian Jewels**: Skill-specific optimizations
- **Cobalt Jewels**: Mine and totem specializations
- **Abyss Jewels**: Socket optimization strategies
- **Cluster Jewels**: Passive point efficiency

#### Flask Optimization
- **Utility Flasks**: Duration and effect optimization
- **Life/Mana Flasks**: Recovery optimization
- **Unique Flasks**: Special flask considerations

## 🏗️ Architecture

### Core Components

```csharp
public abstract class CrafterBase
{
    protected abstract string ItemType { get; }
    protected abstract List<Affix> DesiredAffixes { get; }
    protected abstract CraftingStrategy OptimalStrategy { get; }
    
    public virtual CraftingResult CalculateOptimalPath()
    {
        return new CraftingResult
        {
            Strategy = OptimalStrategy,
            ExpectedCost = CalculateExpectedCost(),
            SuccessRate = CalculateSuccessRate(),
            AverageAttempts = CalculateAverageAttempts()
        };
    }
}
```

### Affix System

```csharp
public class Affix
{
    public string Name { get; set; }
    public AffixType Type { get; set; } // Prefix, Suffix
    public ModTier Tier { get; set; }
    public double Weight { get; set; }
    public List<Affix> BlockedBy { get; set; }
    public ItemLevel RequiredLevel { get; set; }
}

public class AffixLookup
{
    public static List<Affix> GetAvailableAffixes(string itemType, int itemLevel)
    {
        // Return all possible affixes for item type and level
    }
    
    public static double GetWeight(Affix affix, List<Affix> existingAffixes)
    {
        // Calculate weighted probability considering existing affixes
    }
}
```

## 🎮 Usage Examples

### Basic Crafting Analysis

```csharp
// Analyze crafting a high-end weapon
var weaponCrafter = new JeweledFoilCrafter();
var result = weaponCrafter.CalculateOptimalPath();

Console.WriteLine($"Optimal Strategy: {result.Strategy}");
Console.WriteLine($"Expected Cost: {result.ExpectedCost} chaos");
Console.WriteLine($"Success Rate: {result.SuccessRate:P2}");
Console.WriteLine($"Average Attempts: {result.AverageAttempts}");
```

### Specialized Crafting

```csharp
// Energy Shield armor optimization
var esCrafter = new EnergyShieldArmorCrafter
{
    MinEnergyShield = 600,
    RequiredResistances = new[] { "Cold", "Lightning" },
    MinResistanceTotal = 120
};

var strategies = esCrafter.GetAllViableStrategies();
foreach (var strategy in strategies.OrderBy(s => s.ExpectedCost))
{
    Console.WriteLine($"{strategy.Name}: {strategy.ExpectedCost} chaos ({strategy.TimeEstimate})");
}
```

### Jewel Optimization

```csharp
// Arc mine cobalt jewel optimization
var jewelCrafter = new ArcMineCobalt();
var optimization = jewelCrafter.OptimizeForSkill("Arc", "Mines");

Console.WriteLine($"Optimal Affixes: {string.Join(", ", optimization.RecommendedAffixes)}");
Console.WriteLine($"Expected DPS Increase: {optimization.DpsIncrease:F1}%");
Console.WriteLine($"Crafting Cost: {optimization.CraftingCost} chaos");
```

## 📊 Crafting Strategies

### Alt-Regal Method

```csharp
public class AltRegalStrategy : CraftingStrategy
{
    public override CraftingResult Execute(ItemBase item, List<Affix> desiredAffixes)
    {
        var cost = 0;
        var attempts = 0;
        
        // Phase 1: Alt spam for prefix
        var prefixCost = CalculateAltSpamCost(desiredAffixes.Where(a => a.Type == AffixType.Prefix));
        
        // Phase 2: Regal for suffix
        var regalCost = CalculateRegalCost(desiredAffixes.Where(a => a.Type == AffixType.Suffix));
        
        // Phase 3: Craft remaining modifiers
        var craftingCost = CalculateCraftingBenchCost(desiredAffixes);
        
        return new CraftingResult
        {
            TotalCost = prefixCost + regalCost + craftingCost,
            ExpectedAttempts = CalculateExpectedAttempts(),
            TimeEstimate = TimeSpan.FromMinutes(CalculateTimeMinutes())
        };
    }
}
```

### Fossil Crafting

```csharp
public class FossilStrategy : CraftingStrategy
{
    public List<Fossil> OptimalFossils { get; set; }
    
    public override CraftingResult Execute(ItemBase item, List<Affix> desiredAffixes)
    {
        var fossilCombination = CalculateOptimalFossilCombination(desiredAffixes);
        var resonatorCost = GetResonatorCost(fossilCombination.Count);
        var fossilCost = fossilCombination.Sum(f => f.AverageCost);
        
        return new CraftingResult
        {
            Strategy = $"Fossil: {string.Join(" + ", fossilCombination.Select(f => f.Name))}",
            TotalCost = (resonatorCost + fossilCost) * CalculateExpectedAttempts(),
            SuccessRate = CalculateFossilSuccessRate(fossilCombination, desiredAffixes)
        };
    }
}
```

## 🧮 Mathematical Models

### Probability Calculations

```csharp
public static class CraftingMath
{
    public static double CalculateSuccessRate(List<Affix> desired, List<Affix> available)
    {
        var totalWeight = available.Sum(a => a.Weight);
        var desiredWeight = desired.Sum(a => a.Weight);
        
        return desiredWeight / totalWeight;
    }
    
    public static double CalculateGeometricMean(double successRate)
    {
        return 1.0 / successRate; // Expected attempts
    }
    
    public static double CalculateExpectedCost(double successRate, double attemptCost)
    {
        return CalculateGeometricMean(successRate) * attemptCost;
    }
}
```

### Advanced Calculations

```csharp
public class CraftingSimulator
{
    public SimulationResult RunMonteCarlo(CraftingStrategy strategy, int iterations = 10000)
    {
        var results = new List<int>();
        
        for (int i = 0; i < iterations; i++)
        {
            var attempts = SimulateCraftingAttempts(strategy);
            results.Add(attempts);
        }
        
        return new SimulationResult
        {
            AverageAttempts = results.Average(),
            MedianAttempts = results.OrderBy(x => x).Skip(iterations / 2).First(),
            StandardDeviation = CalculateStandardDeviation(results),
            ConfidenceInterval = CalculateConfidenceInterval(results, 0.95)
        };
    }
}
```

## 📁 Project Structure

```
PoeCrafter/
├── Crafters/               # Specialized crafting classes
│   ├── AmuletCrafter.cs
│   ├── ArmorCrafter.cs
│   ├── WeaponCrafter.cs
│   └── JewelCrafter.cs
├── ModGroups/             # Modifier group definitions
│   ├── ArcMineCobalt.cs
│   ├── EleHitBowsViridian.cs
│   └── ModGroupBase.cs
├── Affix.cs              # Affix definitions and utilities
├── AffixLookup.cs        # Affix lookup and filtering
├── AffixParser.cs        # Game data parsing
├── Enums.cs             # Crafting-related enumerations
├── Interfaces.cs        # Crafting interfaces
├── ItemInfoParser.cs    # Item parsing and analysis
├── Prefixes.cs          # Prefix modifier definitions
├── Suffixes.cs          # Suffix modifier definitions
└── Program.cs           # Console application entry point
```

## 🚀 Getting Started

### Running the Application

```bash
# Run with default settings
dotnet run --project PoeCrafter

# Run with specific item type
dotnet run --project PoeCrafter -- --item-type "Jeweled Foil"

# Run simulation mode
dotnet run --project PoeCrafter -- --simulate --iterations 10000
```

### Command Line Options

```bash
# Available options
--item-type <type>        # Specify item type to analyze
--simulate               # Run Monte Carlo simulation
--iterations <count>     # Number of simulation iterations
--league <league>        # League for price data
--budget <chaos>         # Maximum crafting budget
--verbose               # Detailed output
```

## 🧪 Testing

```bash
# Run crafting calculation tests
dotnet test PoeCrafter.Tests/

# Test specific crafter
dotnet test --filter "JeweledFoilCrafterTests"
```

## ⚙️ Configuration

### Item Database

The application uses data files for modifier information:

```json
{
  "ItemTypes": {
    "JeweledFoil": {
      "BaseType": "One Hand Sword",
      "RequiredLevel": 66,
      "ImplicitMods": ["(25-35)% increased Critical Strike Chance"],
      "Tags": ["weapon", "sword", "one_hand"]
    }
  },
  "Modifiers": {
    "WeaponPhysicalDamage": {
      "Tiers": [
        {"Name": "Flaring", "Range": "155-169%", "Weight": 1000, "RequiredLevel": 69},
        {"Name": "Dictator's", "Range": "140-154%", "Weight": 1000, "RequiredLevel": 64}
      ]
    }
  }
}
```

## 📈 Output Examples

### Crafting Analysis Output

```
=== Jeweled Foil Crafting Analysis ===

Target Modifiers:
- T1 Physical Damage (155-169%)
- T1 Attack Speed (25-27%)
- T1 Critical Strike Multiplier (30-32%)

Optimal Strategy: Alt-Regal + Craft
Expected Cost: 2,847 chaos orbs
Success Rate: 0.35%
Average Attempts: 285

Alternative Strategies:
1. Fossil Crafting (Serrated + Prismatic): 3,124 chaos (0.32% success)
2. Chaos Spam: 4,892 chaos (0.20% success)
3. Essence Crafting (Contempt): 2,156 chaos (0.48% success)

Recommended: Essence Crafting for budget efficiency
```

## 🔍 Advanced Features

### Item Quality Analysis

```csharp
public class ItemAnalyzer
{
    public QualityAnalysis AnalyzeItem(ParsedItem item)
    {
        return new QualityAnalysis
        {
            TierAnalysis = AnalyzeTiers(item.Modifiers),
            ImprovementSuggestions = GetImprovementOptions(item),
            MarketValue = EstimateMarketValue(item),
            CraftingPotential = AssessCraftingOptions(item)
        };
    }
}
```

### Batch Processing

```csharp
public class BatchCrafter
{
    public async Task<List<CraftingResult>> AnalyzeMultipleItems(List<string> itemTypes)
    {
        var tasks = itemTypes.Select(async itemType =>
        {
            var crafter = CrafterFactory.GetCrafter(itemType);
            return await crafter.CalculateOptimalPathAsync();
        });
        
        return await Task.WhenAll(tasks);
    }
}
```

## 🤝 Contributing

When contributing to PoeCrafter:

1. **Add New Crafters**: Follow the CrafterBase pattern
2. **Update Mod Data**: Keep modifier databases current
3. **Include Tests**: Add tests for new calculations
4. **Document Strategies**: Explain crafting logic clearly
5. **Validate Math**: Ensure probability calculations are correct

## 🔗 Related Projects

- [PoeLib](../PoeLib/) - Core POE integration library
- [PoeTrade.Contracts](../PoeTrade.Contracts/) - Shared data contracts
- [PoeTradeMonitor.GUI](../PoeTradeMonitor.GUI/) - Main application
- [PoeAuthenticator](../PoeAuthenticator/) - Account authentication