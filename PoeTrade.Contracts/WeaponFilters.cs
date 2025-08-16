using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents weapon-specific filters for Path of Exile trade searches.
/// Contains filters for damage, DPS, critical strike, and attack speed properties.
/// </summary>
public record WeaponFilters
{
    /// <summary>
    /// Gets whether this weapon filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific weapon filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public WeaponFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for weapon properties in trade searches.
/// </summary>
public record WeaponFilterOptions
{
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
    /// Gets the physical damage per second (pDPS) range filter for weapons.
    /// </summary>
    [JsonPropertyName("pdps")]
    public RangeFilter? PhysicalDps { get; init; }
    
    /// <summary>
    /// Gets the elemental damage per second (eDPS) range filter for weapons.
    /// </summary>
    [JsonPropertyName("edps")]
    public RangeFilter? ElementalDps { get; init; }
    
    /// <summary>
    /// Gets the total damage per second (DPS) range filter for weapons.
    /// </summary>
    [JsonPropertyName("dps")]
    public RangeFilter? Dps { get; init; }
    
    /// <summary>
    /// Gets the attacks per second (APS) range filter for weapons.
    /// </summary>
    [JsonPropertyName("aps")]
    public RangeFilter? AttacksPerSecond { get; init; }
}