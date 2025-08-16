using System.Text.Json.Serialization;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents Heist league-specific filters for Path of Exile trade searches.
/// Contains filters for Heist contract and blueprint properties.
/// </summary>
public record HeistFilters
{
    /// <summary>
    /// Gets whether this Heist filter group is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool? Disabled { get; init; }
    
    /// <summary>
    /// Gets the specific Heist filter options.
    /// </summary>
    [JsonPropertyName("filters")]
    public HeistFilterOptions Filters { get; init; } = new();
}

/// <summary>
/// Contains the specific filter options for Heist league content in trade searches.
/// Includes filters for contract properties, blueprint layouts, and required job skills.
/// </summary>
public record HeistFilterOptions
{
    /// <summary>
    /// Gets the number of wings range filter for Heist blueprints.
    /// </summary>
    [JsonPropertyName("heist_wings")]
    public RangeFilter? Wings { get; init; }
    
    /// <summary>
    /// Gets the number of escape routes range filter for Heist blueprints.
    /// </summary>
    [JsonPropertyName("heist_escape_routes")]
    public RangeFilter? EscapeRoutes { get; init; }
    
    /// <summary>
    /// Gets the number of reward rooms range filter for Heist blueprints.
    /// </summary>
    [JsonPropertyName("heist_reward_rooms")]
    public RangeFilter? RewardRooms { get; init; }
    
    /// <summary>
    /// Gets the maximum wings range filter for Heist blueprints.
    /// </summary>
    [JsonPropertyName("heist_max_wings")]
    public RangeFilter? MaxWings { get; init; }
    
    /// <summary>
    /// Gets the maximum escape routes range filter for Heist blueprints.
    /// </summary>
    [JsonPropertyName("heist_max_escape_routes")]
    public RangeFilter? MaxEscapeRoutes { get; init; }
    
    /// <summary>
    /// Gets the maximum reward rooms range filter for Heist blueprints.
    /// </summary>
    [JsonPropertyName("heist_max_reward_rooms")]
    public RangeFilter? MaxRewardRooms { get; init; }
    
    /// <summary>
    /// Gets the objective value filter for Heist contracts.
    /// </summary>
    [JsonPropertyName("heist_objective_value")]
    public OptionFilter? ObjectiveValue { get; init; }
    
    /// <summary>
    /// Gets the lockpicking skill requirement range filter for Heist content.
    /// </summary>
    [JsonPropertyName("heist_lockpicking")]
    public RangeFilter? Lockpicking { get; init; }
    
    /// <summary>
    /// Gets the perception skill requirement range filter for Heist content.
    /// </summary>
    [JsonPropertyName("heist_perception")]
    public RangeFilter? Perception { get; init; }
    
    /// <summary>
    /// Gets the counter-thaumaturgy skill requirement range filter for Heist content.
    /// </summary>
    [JsonPropertyName("heist_counter_thaumaturgy")]
    public RangeFilter? CounterThaumaturgy { get; init; }
    
    /// <summary>
    /// Gets the agility skill requirement range filter for Heist content.
    /// </summary>
    [JsonPropertyName("heist_agility")]
    public RangeFilter? Agility { get; init; }
    
    /// <summary>
    /// Gets the engineering skill requirement range filter for Heist content.
    /// </summary>
    [JsonPropertyName("heist_engineering")]
    public RangeFilter? Engineering { get; init; }
    
    /// <summary>
    /// Gets the deception skill requirement range filter for Heist content.
    /// </summary>
    [JsonPropertyName("heist_deception")]
    public RangeFilter? Deception { get; init; }
    
    /// <summary>
    /// Gets the trap disarmament skill requirement range filter for Heist content.
    /// </summary>
    [JsonPropertyName("heist_trap_disarmament")]
    public RangeFilter? TrapDisarmament { get; init; }
    
    /// <summary>
    /// Gets the demolition skill requirement range filter for Heist content.
    /// </summary>
    [JsonPropertyName("heist_demolition")]
    public RangeFilter? Demolition { get; init; }
    
    /// <summary>
    /// Gets the brute force skill requirement range filter for Heist content.
    /// </summary>
    [JsonPropertyName("heist_brute_force")]
    public RangeFilter? BruteForce { get; init; }
}