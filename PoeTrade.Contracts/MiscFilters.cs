using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents miscellaneous filters for Path of Exile trade searches.
/// Contains filters for various item properties like quality, corruption, identification status, and game-specific features.
/// </summary>
public record MiscFilters
{
    /// <summary>
    /// Gets whether this miscellaneous filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific miscellaneous filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public MiscFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for miscellaneous item properties in trade searches.
/// Supports both PoE1 and PoE2 with game-specific properties clearly marked.
/// </summary>
public record MiscFilterOptions
{
    // Common properties
    /// <summary>
    /// Gets the quality percentage range filter for items.
    /// </summary>
    [JsonPropertyName("quality")]
    public RangeFilter? Quality { get; init; }
    
    /// <summary>
    /// Gets the gem level range filter for skill and support gems.
    /// </summary>
    [JsonPropertyName("gem_level")]
    public RangeFilter? GemLevel { get; init; }
    
    /// <summary>
    /// Gets the item level range filter.
    /// </summary>
    [JsonPropertyName("ilvl")]
    public RangeFilter? ItemLevel { get; init; }
    
    /// <summary>
    /// Gets the area level range filter for maps and other area-based content.
    /// </summary>
    [JsonPropertyName("area_level")]
    public RangeFilter? AreaLevel { get; init; }
    
    /// <summary>
    /// Gets the stack size range filter for stackable items.
    /// </summary>
    [JsonPropertyName("stack_size")]
    public RangeFilter? StackSize { get; init; }
    
    /// <summary>
    /// Gets the corruption status filter (corrupted, not corrupted, any).
    /// </summary>
    [JsonPropertyName("corrupted")]
    public OptionFilter? Corrupted { get; init; }
    
    /// <summary>
    /// Gets the alternate art filter for items with special visual variants.
    /// </summary>
    [JsonPropertyName("alternate_art")]
    public OptionFilter? AlternateArt { get; init; }
    
    /// <summary>
    /// Gets the identification status filter (identified, unidentified, any).
    /// </summary>
    [JsonPropertyName("identified")]
    public OptionFilter? Identified { get; init; }
    
    /// <summary>
    /// Gets the mirrored status filter for items created with Mirror of Kalandra.
    /// </summary>
    [JsonPropertyName("mirrored")]
    public OptionFilter? Mirrored { get; init; }
    
    /// <summary>
    /// Gets the sanctum gold range filter for Sanctum league content.
    /// </summary>
    [JsonPropertyName("sanctum_gold")]
    public RangeFilter? SanctumGold { get; init; }
    
    // PoE1-specific properties
    /// <summary>
    /// Gets the gem level progress percentage range filter (PoE1 only).
    /// </summary>
    [JsonPropertyName("gem_level_progress")]
    public RangeFilter? GemLevelProgress { get; init; }
    
    /// <summary>
    /// Gets the transfigured gem filter for alternate skill gem versions (PoE1 only).
    /// </summary>
    [JsonPropertyName("gem_transfigured")]
    public OptionFilter? GemTransfigured { get; init; }
    
    /// <summary>
    /// Gets the fractured item filter for items with locked modifiers (PoE1 only).
    /// </summary>
    [JsonPropertyName("fractured_item")]
    public OptionFilter? FracturedItem { get; init; }
    
    /// <summary>
    /// Gets the searing item filter for items influenced by Searing Exarch (PoE1 only).
    /// </summary>
    [JsonPropertyName("searing_item")]
    public OptionFilter? SearingItem { get; init; }
    
    /// <summary>
    /// Gets the memory level range filter for Synthesis memories (PoE1 only).
    /// </summary>
    [JsonPropertyName("memory_level")]
    public RangeFilter? MemoryLevel { get; init; }
    
    /// <summary>
    /// Gets the Vaal gem filter for corrupted skill gems with alternate versions (PoE1 only).
    /// </summary>
    [JsonPropertyName("gem_vaal")]
    public OptionFilter? GemVaal { get; init; }
    
    /// <summary>
    /// Gets the synthesised item filter for items from Synthesis league (PoE1 only).
    /// </summary>
    [JsonPropertyName("synthesised_item")]
    public OptionFilter? SynthesisedItem { get; init; }
    
    /// <summary>
    /// Gets the tangled item filter for items influenced by Eater of Worlds (PoE1 only).
    /// </summary>
    [JsonPropertyName("tangled_item")]
    public OptionFilter? TangledItem { get; init; }
    
    /// <summary>
    /// Gets the crafted modifier filter for items with crafted mods (PoE1 only).
    /// </summary>
    [JsonPropertyName("crafted")]
    public OptionFilter? Crafted { get; init; }
    
    /// <summary>
    /// Gets the foreseeing filter for items with prophecy effects (PoE1 only).
    /// </summary>
    [JsonPropertyName("foreseeing")]
    public OptionFilter? Foreseeing { get; init; }
    
    /// <summary>
    /// Gets the split item filter for items created using Beast crafting (PoE1 only).
    /// </summary>
    [JsonPropertyName("split")]
    public OptionFilter? Split { get; init; }
    
    /// <summary>
    /// Gets the veiled modifier filter for items with hidden modifiers (PoE1 only).
    /// </summary>
    [JsonPropertyName("veiled")]
    public OptionFilter? Veiled { get; init; }
    
    /// <summary>
    /// Gets the talisman tier range filter for talisman amulets (PoE1 only).
    /// </summary>
    [JsonPropertyName("talisman_tier")]
    public RangeFilter? TalismanTier { get; init; }
    
    /// <summary>
    /// Gets the stored experience range filter for gems with stored experience (PoE1 only).
    /// </summary>
    [JsonPropertyName("stored_experience")]
    public RangeFilter? StoredExperience { get; init; }
    
    /// <summary>
    /// Gets the corpse type filter for Necropolis league corpses (PoE1 only).
    /// </summary>
    [JsonPropertyName("corpse_type")]
    public OptionFilter? CorpseType { get; init; }
    
    /// <summary>
    /// Gets the crucible item filter for items with passive skill trees (PoE1 only).
    /// </summary>
    [JsonPropertyName("crucible_item")]
    public OptionFilter? CrucibleItem { get; init; }
    
    /// <summary>
    /// Gets the scourge tier range filter for items from Scourge league (PoE1 only).
    /// </summary>
    [JsonPropertyName("scourge_tier")]
    public RangeFilter? ScourgeTier { get; init; }
    
    /// <summary>
    /// Gets the foil variation filter for special visual variants of unique items (PoE1 only).
    /// </summary>
    [JsonPropertyName("foil_variation")]
    public OptionFilter? FoilVariation { get; init; }
    
    // PoE2-specific properties
    /// <summary>
    /// Gets the gem socket count range filter for items (PoE2 only).
    /// </summary>
    [JsonPropertyName("gem_sockets")]
    public RangeFilter? GemSockets { get; init; }
    
    /// <summary>
    /// Gets the unidentified tier range filter for items with tier-based identification (PoE2 only).
    /// </summary>
    [JsonPropertyName("unidentified_tier")]
    public RangeFilter? UnidentifiedTier { get; init; }
}