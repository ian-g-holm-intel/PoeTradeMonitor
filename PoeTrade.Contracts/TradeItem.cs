using PoeTrade.Contracts.Extensions;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PoeTrade.Contracts;

/// <summary>
/// Represents a tradeable item in Path of Exile with all its properties and metadata.
/// Contains comprehensive information about the item including mods, sockets, properties, and trading details.
/// </summary>
public record TradeItem
{
    /// <summary>
    /// Gets the game realm where this item exists (e.g., "pc", "xbox", "sony").
    /// Can be null if realm information is not available.
    /// </summary>
    [JsonPropertyName("realm")]
    public string? Realm { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether this item listing has been verified by the trade system.
    /// Verified items have been confirmed to exist in the seller's inventory.
    /// </summary>
    [JsonPropertyName("verified")]
    public bool Verified { get; init; }
    
    /// <summary>
    /// Gets the width of the item in inventory grid units.
    /// Used for inventory management and display purposes.
    /// </summary>
    [JsonPropertyName("w")]
    public int Width { get; init; }
    
    /// <summary>
    /// Gets the height of the item in inventory grid units.
    /// Used for inventory management and display purposes.
    /// </summary>
    [JsonPropertyName("h")]
    public int Height { get; init; }
    
    /// <summary>
    /// Gets the URL or path to the item's icon image.
    /// Used for displaying the item in user interfaces.
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets a value indicating whether this is a support gem item.
    /// Can be null if support gem status is not applicable or unknown.
    /// </summary>
    [JsonPropertyName("support")]
    public bool? Support { get; init; }
    
    /// <summary>
    /// Gets the current stack size of the item.
    /// For stackable items like currency, this represents how many are in the stack.
    /// </summary>
    [JsonPropertyName("stackSize")]
    public int StackSize { get; init; } = 1;
    
    /// <summary>
    /// Gets the maximum allowed stack size for this item type.
    /// Can be null for non-stackable items.
    /// </summary>
    [JsonPropertyName("maxStackSize")]
    public int? MaxStackSize { get; init; }
    
    /// <summary>
    /// Gets the league where this item exists (e.g., "Hardcore", "Standard", "Ritual").
    /// Essential for filtering trades by game mode.
    /// </summary>
    [JsonPropertyName("league")]
    public string League { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the unique identifier for this item instance.
    /// Used for tracking and fetching specific items from the trade API.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the search identifier associated with this item.
    /// Used internally for tracking which search query returned this item.
    /// This property is not serialized to JSON.
    /// </summary>
    [JsonIgnore]
    public string SearchID { get; set; } = string.Empty;

    /// <summary>
    /// Gets the list of sockets and their properties for this item.
    /// Contains information about socket colors, links, and positions.
    /// Can be null for items that don't have sockets.
    /// </summary>
    [JsonPropertyName("sockets")]
    public List<SocketInfo>? Sockets { get; init; }

    /// <summary>
    /// Gets the maximum number of linked sockets in the largest link group.
    /// </summary>
    [JsonIgnore]
    public int MaxLinks
    {
        get
        {
            if (Sockets == null || Sockets.Count == 0) return 0;
            return Sockets.GroupBy(x => x.Group).Select(g => g.Count()).Max();
        }
    }

    /// <summary>
    /// Gets the quality percentage of the item, parsed from the item's properties.
    /// Quality affects various item statistics and is displayed as a percentage.
    /// Returns 0 if the item has no quality property.
    /// </summary>
    [JsonIgnore]
    public int Quality
    {
        get
        {
            var qualityProperty = Properties?.FirstOrDefault(p => p.Name == "[Quality]");
            if (qualityProperty != null)
            {
                var value = qualityProperty.Values?.FirstOrDefault();
                if (value != null && value.Value != null)
                {
                    return int.Parse(value.Value.Trim('+', '%'));
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// Gets the gem level of the item, parsed from the item's properties.
    /// Only applicable to skill gems and support gems.
    /// Returns 0 if the item is not a gem or has no level property.
    /// </summary>
    [JsonIgnore]
    public int GemLevel
    {
        get
        {
            var levelProperty = Properties?.FirstOrDefault(p => p.Name == "Level");
            if (levelProperty != null)
            {
                var value = levelProperty.Values?.FirstOrDefault();
                if (value != null && value.Value != null)
                {
                    return int.Parse(value.Value);
                }
            }
            return 0;
        }
    }

    private static Regex cleanupPattern = new Regex(@"<<[\w:]*>>", RegexOptions.Compiled);
    private string name = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the item.
    /// When setting, removes markup, prefixes like "Superior" and "Synthesised", and normalizes the text.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name
    {
        get => string.IsNullOrEmpty(name) ? TypeLine : name;
        set => name = cleanupPattern.Replace(value, "").Replace("Superior", "").Replace("Synthesised", "").RemoveDiacritics().Trim();
    }


    /// <summary>
    /// Gets the type line text of the item as it appears in game.
    /// This is the base type or category description of the item.
    /// </summary>
    [JsonPropertyName("typeLine")]
    public string TypeLine { get; init; } = string.Empty;
    
    /// <summary>
    /// Gets the base type of the item without any modifications.
    /// Used for filtering and categorization purposes.
    /// </summary>
    [JsonPropertyName("baseType")]
    public string BaseType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the item rarity based on the frame type.
    /// </summary>
    [JsonIgnore]
    public ItemRarity Rarity => (ItemRarity)FrameType;

    /// <summary>
    /// Gets the item level (ilvl) which determines what modifiers can appear on the item.
    /// Higher item levels allow for more powerful modifiers.
    /// </summary>
    [JsonPropertyName("ilvl")]
    public int ItemLevel { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the item has been identified.
    /// Unidentified items hide their modifiers and exact properties.
    /// </summary>
    [JsonPropertyName("identified")]
    public bool Identified { get; init; }
    
    /// <summary>
    /// Gets the seller's note or price comment for the item listing.
    /// Can be null if no note was provided by the seller.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the item is corrupted.
    /// Corrupted items cannot be modified further and may have special properties.
    /// Can be null if corruption status is not applicable.
    /// </summary>
    [JsonPropertyName("corrupted")]
    public bool? Corrupted { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the item is synthesised.
    /// Synthesised items have special implicit modifiers from the Synthesis league.
    /// Can be null if synthesis status is not applicable.
    /// </summary>
    [JsonPropertyName("synthesised")]
    public bool? Synthesised { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the item has been duplicated.
    /// Duplicated items are copies created through certain game mechanics.
    /// Can be null if duplication status is not applicable.
    /// </summary>
    [JsonPropertyName("duplicated")]
    public bool? Duplicated { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the item has been split.
    /// Split items cannot be split again and have reduced value in some contexts.
    /// Can be null if split status is not applicable.
    /// </summary>
    [JsonPropertyName("split")]
    public bool? Split { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the item has fractured modifiers.
    /// Fractured modifiers are locked and cannot be changed through crafting.
    /// Can be null if fracture status is not applicable.
    /// </summary>
    [JsonPropertyName("fractured")]
    public bool? Fractured { get; init; }
    
    /// <summary>
    /// Gets a value indicating whether the item is a relic from legacy leagues.
    /// Relic items have special visual effects and may have legacy modifiers.
    /// Can be null if relic status is not applicable.
    /// </summary>
    [JsonPropertyName("isRelic")]
    public bool? IsRelic { get; init; }
    
    /// <summary>
    /// Gets the foil variation number for special visual effects on the item.
    /// Used for unique items with alternate art or special presentations.
    /// Can be null if no foil variation is present.
    /// </summary>
    [JsonPropertyName("foilVariation")]
    public int? FoilVariation { get; init; }
    
    /// <summary>
    /// Gets the list of item properties such as damage, armor, attack speed, etc.
    /// These are the base statistics and characteristics of the item.
    /// Can be null if the item has no properties.
    /// </summary>
    [JsonPropertyName("properties")]
    public List<ItemProperty>? Properties { get; init; }
    
    /// <summary>
    /// Gets the list of requirements needed to use the item (level, strength, dexterity, intelligence).
    /// Players must meet these requirements to equip or use the item.
    /// Can be null if the item has no requirements.
    /// </summary>
    [JsonPropertyName("requirements")]
    public List<ItemProperty>? Requirements { get; init; }
    
    /// <summary>
    /// Gets the list of requirements for support gems socketed in this item.
    /// Only applicable to items that can have support gems.
    /// Can be null if not applicable or no support gem requirements exist.
    /// </summary>
    [JsonPropertyName("supportGemRequirements")]
    public List<ItemProperty>? SupportGemRequirements { get; init; }
    
    /// <summary>
    /// Gets the list of skills granted by this item.
    /// Some items like gems or unique items can grant special skills to the character.
    /// Can be null if the item grants no skills.
    /// </summary>
    [JsonPropertyName("grantedSkills")]
    public List<GrantedSkill>? GrantedSkills { get; init; }
    
    /// <summary>
    /// Gets the list of enchantment modifiers on the item.
    /// Enchantments are special modifiers applied through the Labyrinth or other endgame content.
    /// Can be null if the item has no enchantments.
    /// </summary>
    [JsonPropertyName("enchantMods")]
    public List<string>? EnchantMods { get; init; }
    
    /// <summary>
    /// Gets the list of implicit modifiers on the item.
    /// Implicit modifiers are inherent to the item type and cannot be changed through normal crafting.
    /// Can be null if the item has no implicit modifiers.
    /// </summary>
    [JsonPropertyName("implicitMods")]
    [JsonConverter(typeof(ModListConverter))]
    public List<Mod>? ImplicitMods { get; init; }
    
    /// <summary>
    /// Gets the list of explicit modifiers on the item.
    /// Explicit modifiers are added through crafting, drops, or other item modifications.
    /// Can be null if the item has no explicit modifiers.
    /// </summary>
    [JsonPropertyName("explicitMods")]
    [JsonConverter(typeof(ModListConverter))]
    public List<Mod>? ExplicitMods { get; init; }
    
    /// <summary>
    /// Gets the list of crafted modifiers on the item.
    /// Crafted modifiers are explicitly added by the player using crafting benches or masters.
    /// Can be null if the item has no crafted modifiers.
    /// </summary>
    [JsonPropertyName("craftedMods")]
    public List<string>? CraftedMods { get; init; }
    
    /// <summary>
    /// Gets the list of rune modifiers on the item (Path of Exile 2 specific).
    /// Rune modifiers are special enhancements available in Path of Exile 2.
    /// Can be null if the item has no rune modifiers or not applicable.
    /// </summary>
    [JsonPropertyName("runeMods")]
    public List<string>? RuneMods { get; init; }
    
    /// <summary>
    /// Gets the list of fractured modifiers on the item.
    /// Fractured modifiers are locked and cannot be changed through normal crafting methods.
    /// Can be null if the item has no fractured modifiers.
    /// </summary>
    [JsonPropertyName("fracturedMods")]
    [JsonConverter(typeof(ModListConverter))]
    public List<Mod>? FracturedMods { get; init; }
    
    /// <summary>
    /// Gets the flavor text lines of the item.
    /// Flavor text provides lore and background information about the item.
    /// Can be null if the item has no flavor text.
    /// </summary>
    [JsonPropertyName("flavourText")]
    public List<string>? FlavourText { get; init; }
    
    /// <summary>
    /// Gets the frame type which determines the item's rarity and visual presentation.
    /// Frame type corresponds to ItemRarity enum values (0=Normal, 1=Magic, 2=Rare, 3=Unique, etc.).
    /// </summary>
    [JsonPropertyName("frameType")]
    public int FrameType { get; init; }
    
    /// <summary>
    /// Gets extended item information including base percentiles and advanced statistics.
    /// Contains additional calculated values used for item evaluation and comparison.
    /// Can be null if extended information is not available.
    /// </summary>
    [JsonPropertyName("extended")]
    [JsonConverter(typeof(ItemExtendedConverter))]
    public ItemExtended? Extended { get; init; }
    
    /// <summary>
    /// Gets the list of items socketed within this item.
    /// Typically gems socketed in weapons, armor, or other socketable items.
    /// Can be null if the item has no socketed items.
    /// </summary>
    [JsonPropertyName("socketedItems")]
    public List<TradeItem>? SocketedItems { get; init; }
    
    /// <summary>
    /// Gets the secondary description text for the item.
    /// Additional descriptive text that may appear below the main item information.
    /// Can be null if no secondary description is present.
    /// </summary>
    [JsonPropertyName("secDescrText")]
    public string? SecondaryDescriptionText { get; init; }
    
    /// <summary>
    /// Gets the list of gem tabs associated with this item (Path of Exile 2 specific).
    /// Contains information about gem configurations and statistics in PoE2.
    /// Can be null if not applicable or no gem tabs are present.
    /// </summary>
    [JsonPropertyName("gemTabs")]
    public List<GemTab>? GemTabs { get; init; }
    
    /// <summary>
    /// Gets the main description text for the item.
    /// Contains detailed description about the item's function or background.
    /// Can be null if no description text is present.
    /// </summary>
    [JsonPropertyName("descrText")]
    public string? DescriptionText { get; init; }
    
    /// <summary>
    /// Gets the socket index if this item is socketed within another item.
    /// Indicates the position of this item in its parent container.
    /// Can be null if the item is not socketed or position is not specified.
    /// </summary>
    [JsonPropertyName("socket")]
    public int? Socket { get; init; }

    /// <summary>
    /// Gets or sets the whisper token used for trade communication.
    /// This property is not serialized to JSON.
    /// </summary>
    [JsonIgnore]
    public string? WhisperToken { get; set; }

    public override string ToString()
    {
        return Name;
    }
}