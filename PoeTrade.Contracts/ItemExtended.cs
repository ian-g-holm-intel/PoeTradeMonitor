using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents extended item information with calculated properties and metadata.
/// Contains computed values like DPS, defense percentiles, and augmentation flags.
/// </summary>
public record ItemExtended
{
    /// <summary>
    /// Gets the base defense percentile ranking of the item.
    /// </summary>
    [JsonPropertyName("base_defence_percentile")]
    public int? BaseDefencePercentile { get; init; }
    
    /// <summary>
    /// Gets the calculated evasion rating of the item.
    /// </summary>
    [JsonPropertyName("ev")]
    public int? Evasion { get; init; }
    
    /// <summary>
    /// Gets whether the evasion value has been augmented by modifiers.
    /// </summary>
    [JsonPropertyName("ev_aug")]
    public bool? EvasionAugmented { get; init; }
    
    /// <summary>
    /// Gets the calculated armor rating of the item.
    /// </summary>
    [JsonPropertyName("ar")]
    public int? Armour { get; init; }
    
    /// <summary>
    /// Gets whether the armor value has been augmented by modifiers.
    /// </summary>
    [JsonPropertyName("ar_aug")]
    public bool? ArmourAugmented { get; init; }
    
    /// <summary>
    /// Gets the calculated energy shield value of the item.
    /// </summary>
    [JsonPropertyName("es")]
    public int? EnergyShield { get; init; }
    
    /// <summary>
    /// Gets whether the energy shield value has been augmented by modifiers.
    /// </summary>
    [JsonPropertyName("es_aug")]
    public bool? EnergyShieldAugmented { get; init; }
    
    /// <summary>
    /// Gets the calculated total damage per second (DPS) of the weapon.
    /// </summary>
    [JsonPropertyName("dps")]
    public decimal? Dps { get; init; }
    
    /// <summary>
    /// Gets the calculated physical damage per second (pDPS) of the weapon.
    /// </summary>
    [JsonPropertyName("pdps")]
    public decimal? PhysicalDps { get; init; }
    
    /// <summary>
    /// Gets the calculated elemental damage per second (eDPS) of the weapon.
    /// </summary>
    [JsonPropertyName("edps")]
    public decimal? ElementalDps { get; init; }
    
    /// <summary>
    /// Gets whether the DPS value has been augmented by modifiers.
    /// </summary>
    [JsonPropertyName("dps_aug")]
    public bool? DpsAugmented { get; init; }
    
    /// <summary>
    /// Gets whether the physical DPS value has been augmented by modifiers.
    /// </summary>
    [JsonPropertyName("pdps_aug")]
    public bool? PhysicalDpsAugmented { get; init; }
    
    /// <summary>
    /// Gets the modifier information for the item.
    /// </summary>
    [JsonPropertyName("mods")]
    public ModsInfo? Mods { get; init; }
    
    /// <summary>
    /// Gets the hash information for the item's modifiers.
    /// </summary>
    [JsonPropertyName("hashes")]
    public HashesInfo? Hashes { get; init; }
    
    /// <summary>
    /// Gets the textual representation of the item.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}