namespace PoeTrade.Contracts;

/// <summary>
/// Represents the rarity tiers of items in Path of Exile.
/// Each rarity has distinct visual and gameplay characteristics.
/// </summary>
public enum ItemRarity
{
    /// <summary>
    /// Normal rarity items with white text color.
    /// Basic items with no magical properties beyond their base type.
    /// </summary>
    Normal,

    /// <summary>
    /// Magic rarity items with blue text color.
    /// Can have 1-2 random magical modifiers (affixes).
    /// </summary>
    Magic,

    /// <summary>
    /// Rare rarity items with yellow text color.
    /// Can have 3-6 random magical modifiers, making them potentially very powerful.
    /// </summary>
    Rare,

    /// <summary>
    /// Unique rarity items with orange/brown text color.
    /// Have fixed, predetermined modifiers that are often build-enabling or thematically interesting.
    /// </summary>
    Unique,

    /// <summary>
    /// Skill and support gems that provide active abilities and passive enhancements.
    /// Essential for character builds and progression.
    /// </summary>
    Gem,

    /// <summary>
    /// Currency items used for crafting, trading, and item modification.
    /// Includes orbs, scrolls, and other consumable items that modify equipment.
    /// </summary>
    Currency,

    /// <summary>
    /// Divination cards that can be collected and exchanged for specific rewards.
    /// Allow targeted farming of particular items or currency.
    /// </summary>
    DivinationCard,

    /// <summary>
    /// Items that are part of quest objectives or storyline progression.
    /// Cannot be traded and are typically temporary or single-use.
    /// </summary>
    QuestItem,

    /// <summary>
    /// Prophecy items that predict and influence future events or encounters.
    /// Can be sealed and traded, providing various effects when triggered.
    /// </summary>
    Prophecy,

    /// <summary>
    /// Foil versions of items with special visual effects and rarity.
    /// Cosmetically enhanced versions of existing items, often from events or promotions.
    /// </summary>
    Foil,

    /// <summary>
    /// Foil versions of unique items with enhanced visual effects.
    /// Combines the special properties of unique items with foil visual enhancements.
    /// </summary>
    UniqueFoil,

    /// <summary>
    /// Items related to the Necropolis league mechanic.
    /// Special category for league-specific content and mechanics.
    /// </summary>
    Necropolis
}