using Google.Protobuf.WellKnownTypes;
using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a trade search request sent to the Path of Exile trade API.
/// Supports both Path of Exile 1 and Path of Exile 2 through nullable filter properties.
/// </summary>
public record TradeSearchRequest
{
    /// <summary>
    /// Gets the search query parameters including filters and criteria.
    /// </summary>
    [JsonPropertyName("query")]
    public TradeQuery Query { get; init; } = new();
    
    /// <summary>
    /// Gets the sort options for the search results.
    /// </summary>
    [JsonPropertyName("sort")]
    public SortOptions Sort { get; init; } = new();

    [JsonIgnore]
    public string Name => string.IsNullOrEmpty(Query.Name) ? Query.Type ?? "Unknown" : Query.Name;

    public override string ToString()
    {
        return Name;
    }
}

/// <summary>
/// Represents the query portion of a trade search, containing filters and search criteria.
/// </summary>
public record TradeQuery
{
    /// <summary>
    /// Name of the Item
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Type of Item
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Player status filter (e.g., "online", "offline").
    /// </summary>
    [JsonPropertyName("status")]
    public StatusFilter Status { get; init; } = new();
    
    /// <summary>
    /// List of stat filters for item properties and modifiers.
    /// </summary>
    [JsonPropertyName("stats")]
    public List<StatsFilter> Stats { get; init; } = [];
    
    /// <summary>
    /// Collection of search filters organized by category.
    /// </summary>
    [JsonPropertyName("filters")]
    public QueryFilters Filters { get; init; } = new();

    public override string ToString()
    {
        return $"{(Name ?? Type ?? string.Empty)}";
    }
}

/// <summary>
/// Represents a filter for player online status.
/// </summary>
public record StatusFilter
{
    /// <summary>
    /// Gets the status option (e.g., "online", "offline", "any").
    /// </summary>
    [JsonPropertyName("option")]
    public string Option { get; init; } = string.Empty;
}

/// <summary>
/// Represents a filter for item statistics and modifiers.
/// </summary>
public record StatsFilter
{
    /// <summary>
    /// Gets the type of stat filter (e.g., "and", "or", "if", "count").
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the list of individual stat filter criteria.
    /// </summary>
    [JsonPropertyName("filters")]
    public List<object> Filters { get; init; } = [];

    /// <summary>
    /// Gets or sets whether this stat filter is currently disabled.
    /// When true, the filter will be ignored in the search query.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
}

/// <summary>
/// Represents an individual stat filter criterion within a StatsFilter.
/// Used to filter items based on specific modifiers, properties, or statistics.
/// </summary>
public record StatFilter
{
    /// <summary>
    /// Gets or sets the unique identifier for the stat being filtered.
    /// This corresponds to the stat ID from the Path of Exile trade API.
    /// </summary>
    [JsonPropertyName("id")]
    public string? id { get; set; }

    /// <summary>
    /// Gets or sets the value range or criteria for the stat filter.
    /// Defines the minimum/maximum values or specific conditions the stat must meet.
    /// </summary>
    [JsonPropertyName("value")]
    public Value value { get; set; } = new();

    /// <summary>
    /// Gets or sets whether this stat filter is currently disabled.
    /// When true, the filter will be ignored in the search query.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; set; }
}

/// <summary>
/// Represents sort options for trade search results.
/// </summary>
public record SortOptions
{
    /// <summary>
    /// Gets the price sort order ("asc" or "desc").
    /// </summary>
    [JsonPropertyName("price")]
    public string? Price { get; init; }
}

/// <summary>
/// Contains all available search filters organized by category.
/// Supports both PoE1 and PoE2 through nullable properties.
/// </summary>
public record QueryFilters
{
    /// <summary>
    /// Gets the type filters for item category, rarity, and level (common to both PoE1 and PoE2).
    /// </summary>
    [JsonPropertyName("type_filters")]
    public TypeFilters? TypeFilters { get; init; }
    
    // PoE1-specific filters
    
    /// <summary>
    /// Gets the weapon-specific filters (PoE1 only). Includes damage, DPS, and attack speed filters.
    /// </summary>
    [JsonPropertyName("weapon_filters")]
    public WeaponFilters? WeaponFilters { get; init; }
    
    /// <summary>
    /// Gets the armor-specific filters (PoE1 only). Includes life, energy shield, and resistance filters.
    /// </summary>
    [JsonPropertyName("armour_filters")]
    public ArmourFilters? ArmourFilters { get; init; }
    
    /// <summary>
    /// Gets the socket filters (PoE1 only). Includes socket number, color, and link requirements.
    /// </summary>
    [JsonPropertyName("socket_filters")]
    public SocketFilters? SocketFilters { get; init; }
    
    /// <summary>
    /// Gets the Heist league-specific filters (PoE1 only).
    /// </summary>
    [JsonPropertyName("heist_filters")]
    public HeistFilters? HeistFilters { get; init; }
    
    /// <summary>
    /// Gets the Sanctum league-specific filters (PoE1 only).
    /// </summary>
    [JsonPropertyName("sanctum_filters")]
    public SanctumFilters? SanctumFilters { get; init; }
    
    /// <summary>
    /// Gets the Ultimatum league-specific filters (PoE1 only).
    /// </summary>
    [JsonPropertyName("ultimatum_filters")]
    public UltimatumFilters? UltimatumFilters { get; init; }
    
    // PoE2-specific filters
    
    /// <summary>
    /// Gets the equipment filters (PoE2 only). Includes rune sockets, spirit, and reload time.
    /// </summary>
    [JsonPropertyName("equipment_filters")]
    public EquipmentFilters? EquipmentFilters { get; init; }
    
    // Common filters
    
    /// <summary>
    /// Gets the requirement filters for level and attribute requirements.
    /// </summary>
    [JsonPropertyName("req_filters")]
    public RequirementFilters? ReqFilters { get; init; }
    
    /// <summary>
    /// Gets the map-specific filters for tier, quality, and map modifiers.
    /// </summary>
    [JsonPropertyName("map_filters")]
    public MapFilters? MapFilters { get; init; }
    
    /// <summary>
    /// Gets the miscellaneous filters for various item properties.
    /// </summary>
    [JsonPropertyName("misc_filters")]
    public MiscFilters? MiscFilters { get; init; }
    
    /// <summary>
    /// Gets the trade-specific filters for price, account, and listing settings.
    /// </summary>
    [JsonPropertyName("trade_filters")]
    public TradeFilters? TradeFilters { get; init; }
}