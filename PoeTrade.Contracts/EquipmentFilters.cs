using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents equipment-specific filters for Path of Exile 2 trade searches.
/// Contains filters for both weapon and armor properties in a unified filter system.
/// </summary>
public record EquipmentFilters
{
    /// <summary>
    /// Gets whether this equipment filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific equipment filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public EquipmentFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for equipment properties in Path of Exile 2 trade searches.
/// Supports both weapon and armor filtering in a unified system.
/// </summary>
public record EquipmentFilterOptions
{
    // Weapon properties
    /// <summary>
    /// Gets the attacks per second range filter for weapons.
    /// </summary>
    [JsonPropertyName("aps")]
    public RangeFilter? AttacksPerSecond { get; init; }
    
    /// <summary>
    /// Gets the total damage per second range filter for weapons.
    /// </summary>
    [JsonPropertyName("dps")]
    public RangeFilter? Dps { get; init; }
    
    /// <summary>
    /// Gets the elemental damage per second range filter for weapons.
    /// </summary>
    [JsonPropertyName("edps")]
    public RangeFilter? ElementalDps { get; init; }
    
    /// <summary>
    /// Gets the damage range filter for weapons.
    /// </summary>
    [JsonPropertyName("damage")]
    public RangeFilter? Damage { get; init; }
    
    /// <summary>
    /// Gets the critical strike chance range filter for weapons.
    /// </summary>
    [JsonPropertyName("crit")]
    public RangeFilter? Crit { get; init; }
    
    /// <summary>
    /// Gets the physical damage per second range filter for weapons.
    /// </summary>
    [JsonPropertyName("pdps")]
    public RangeFilter? PhysicalDps { get; init; }
    
    /// <summary>
    /// Gets the reload time range filter for weapons (PoE2 specific).
    /// </summary>
    [JsonPropertyName("reload_time")]
    public RangeFilter? ReloadTime { get; init; }
    
    // Armour properties
    /// <summary>
    /// Gets the armor rating range filter.
    /// </summary>
    [JsonPropertyName("ar")]
    public RangeFilter? Armour { get; init; }
    
    /// <summary>
    /// Gets the energy shield range filter.
    /// </summary>
    [JsonPropertyName("es")]
    public RangeFilter? EnergyShield { get; init; }
    
    /// <summary>
    /// Gets the spirit range filter (PoE2 specific).
    /// </summary>
    [JsonPropertyName("spirit")]
    public RangeFilter? Spirit { get; init; }
    
    /// <summary>
    /// Gets the evasion rating range filter.
    /// </summary>
    [JsonPropertyName("ev")]
    public RangeFilter? Evasion { get; init; }
    
    /// <summary>
    /// Gets the block chance range filter for shields.
    /// </summary>
    [JsonPropertyName("block")]
    public RangeFilter? Block { get; init; }
    
    /// <summary>
    /// Gets the rune socket count range filter (PoE2 specific).
    /// </summary>
    [JsonPropertyName("rune_sockets")]
    public RangeFilter? RuneSockets { get; init; }
}