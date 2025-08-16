using System.ComponentModel;

namespace PoeTrade.Contracts;

/// <summary>
/// Attribute used to associate trade API tags with currency enum values.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class TradeCurrencyTypeAttribute : Attribute
{
    /// <summary>
    /// Gets the trade API tag for this currency type.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeCurrencyTypeAttribute"/> class.
    /// </summary>
    /// <param name="tag">The trade API tag for the currency.</param>
    public TradeCurrencyTypeAttribute(string tag)
    {
        Tag = tag;
    }
}

/// <summary>
/// Attribute used to specify the maximum stack size for currency types.
/// </summary>
public class StackSizeAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the maximum stack size for the currency.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StackSizeAttribute"/> class.
    /// </summary>
    /// <param name="size">The maximum stack size for the currency.</param>
    public StackSizeAttribute(int size)
    {
        Size = size;
    }
}

/// <summary>
/// Enumeration of all currency types available in Path of Exile trading.
/// Each currency type includes description and trade API tag attributes.
/// </summary>
public enum TradeCurrencyType
{
    [Description("Unknown Currency Type")]
    [TradeCurrencyType("unknown")]
    Unknown = 0,

    [Description("Orb of Alteration")]
    [TradeCurrencyType("alt")]
    Alt,

    [Description("Orb of Fusing")]
    [TradeCurrencyType("fusing")]
    Fusing,

    [Description("Orb of Alchemy")]
    [TradeCurrencyType("alch")]
    Alch,

    [Description("Chaos Orb")]
    [TradeCurrencyType("chaos")]
    [StackSize(20)]
    Chaos,

    [Description("Gemcutter's Prism")]
    [TradeCurrencyType("gcp")]
    Gcp,

    [Description("Exalted Orb")]
    [TradeCurrencyType("exalted")]
    Exalted,

    [Description("Chromatic Orb")]
    [TradeCurrencyType("chrome")]
    Chrome,

    [Description("Jeweller's Orb")]
    [TradeCurrencyType("jewellers")]
    Jewellers,

    [Description("Engineer's Orb")]
    [TradeCurrencyType("engineers")]
    Engineers,

    [Description("Infused Engineer's Orb")]
    [TradeCurrencyType("infused-engineers-orb")]
    InfusedEngineersOrb,

    [Description("Orb of Chance")]
    [TradeCurrencyType("chance")]
    Chance,

    [Description("Cartographer's Chisel")]
    [TradeCurrencyType("chisel")]
    Chisel,

    [Description("Orb of Scouring")]
    [TradeCurrencyType("scour")]
    Scour,

    [Description("Blessed Orb")]
    [TradeCurrencyType("blessed")]
    Blessed,

    [Description("Orb of Regret")]
    [TradeCurrencyType("regret")]
    Regret,

    [Description("Regal Orb")]
    [TradeCurrencyType("regal")]
    Regal,

    [Description("Divine Orb")]
    [TradeCurrencyType("divine")]
    [StackSize(20)]
    Divine,

    [Description("Vaal Orb")]
    [TradeCurrencyType("vaal")]
    Vaal,

    [Description("Orb of Annulment")]
    [TradeCurrencyType("annul")]
    Annul,

    [Description("Orb of Binding")]
    [TradeCurrencyType("orb-of-binding")]
    OrbOfBinding,

    [Description("Ancient Orb")]
    [TradeCurrencyType("ancient-orb")]
    AncientOrb,

    [Description("Orb of Horizons")]
    [TradeCurrencyType("orb-of-horizons")]
    OrbOfHorizons,

    [Description("Harbinger's Orb")]
    [TradeCurrencyType("harbingers-orb")]
    HarbingersOrb,

    [Description("Fracturing Orb")]
    [TradeCurrencyType("fracturing-orb")]
    FracturingOrb,

    [Description("Scroll of Wisdom")]
    [TradeCurrencyType("wisdom")]
    Wisdom,

    [Description("Portal Scroll")]
    [TradeCurrencyType("portal")]
    Portal,

    [Description("Armourer's Scrap")]
    [TradeCurrencyType("scrap")]
    Scrap,

    [Description("Blacksmith's Whetstone")]
    [TradeCurrencyType("whetstone")]
    Whetstone,

    [Description("Glassblower's Bauble")]
    [TradeCurrencyType("bauble")]
    Bauble,

    [Description("Orb of Transmutation")]
    [TradeCurrencyType("transmute")]
    Transmute,

    [Description("Orb of Augmentation")]
    [TradeCurrencyType("aug")]
    Aug,

    [Description("Mirror of Kalandra")]
    [TradeCurrencyType("mirror")]
    Mirror,

    [Description("Eternal Orb")]
    [TradeCurrencyType("eternal")]
    Eternal,

    [Description("Rogue's Marker")]
    [TradeCurrencyType("rogues-marker")]
    RoguesMarker,

    [Description("Facetor's Lens")]
    [TradeCurrencyType("facetors")]
    Facetors,

    [Description("Tempering Orb")]
    [TradeCurrencyType("tempering-orb")]
    TemperingOrb,

    [Description("Tailoring Orb")]
    [TradeCurrencyType("tailoring-orb")]
    TailoringOrb,

    [Description("Orb of Unmaking")]
    [TradeCurrencyType("orb-of-unmaking")]
    OrbOfUnmaking,

    [Description("Veiled Chaos Orb")]
    [TradeCurrencyType("veiled-chaos-orb")]
    VeiledChaosOrb,

    [Description("Veiled Exalted Orb")]
    [TradeCurrencyType("veiled-exalted-orb")]
    VeiledExaltedOrb,

    [Description("Enkindling Orb")]
    [TradeCurrencyType("enkindling-orb")]
    EnkindlingOrb,

    [Description("Instilling Orb")]
    [TradeCurrencyType("instilling-orb")]
    InstillingOrb,

    [Description("Sacred Orb")]
    [TradeCurrencyType("sacred-orb")]
    SacredOrb,

    [Description("Stacked Deck")]
    [TradeCurrencyType("stacked-deck")]
    StackedDeck,

    [Description("Veiled Scarab")]
    [TradeCurrencyType("veiled-scarab")]
    VeiledScarab,

    [Description("Shaper's Exalted Orb")]
    [TradeCurrencyType("shapers-exalted-orb")]
    ShapersExaltedOrb,

    [Description("Elder's Exalted Orb")]
    [TradeCurrencyType("elders-exalted-orb")]
    EldersExaltedOrb,

    [Description("Crusader's Exalted Orb")]
    [TradeCurrencyType("crusaders-exalted-orb")]
    CrusadersExaltedOrb,

    [Description("Redeemer's Exalted Orb")]
    [TradeCurrencyType("redeemers-exalted-orb")]
    RedeemersExaltedOrb,

    [Description("Hunter's Exalted Orb")]
    [TradeCurrencyType("hunters-exalted-orb")]
    HuntersExaltedOrb,

    [Description("Warlord's Exalted Orb")]
    [TradeCurrencyType("warlords-exalted-orb")]
    WarlordsExaltedOrb,

    [Description("Awakener's Orb")]
    [TradeCurrencyType("awakeners-orb")]
    AwakenersOrb,

    [Description("Orb of Dominance")]
    [TradeCurrencyType("mavens-orb")]
    MavensOrb,

    [Description("Orb of Remembrance")]
    [TradeCurrencyType("orb-of-remembrance")]
    OrbOfRemembrance,

    [Description("Orb of Unravelling")]
    [TradeCurrencyType("orb-of-unravelling")]
    OrbOfUnravelling,

    [Description("Orb of Intention")]
    [TradeCurrencyType("orb-of-intention")]
    OrbOfIntention,

    [Description("Eldritch Chaos Orb")]
    [TradeCurrencyType("eldritch-chaos-orb")]
    EldritchChaosOrb,

    [Description("Eldritch Exalted Orb")]
    [TradeCurrencyType("eldritch-exalted-orb")]
    EldritchExaltedOrb,

    [Description("Eldritch Orb of Annulment")]
    [TradeCurrencyType("eldritch-orb-of-annulment")]
    EldritchOrbOfAnnulment,

    [Description("Lesser Eldritch Ember")]
    [TradeCurrencyType("lesser-eldritch-ember")]
    LesserEldritchEmber,

    [Description("Greater Eldritch Ember")]
    [TradeCurrencyType("greater-eldritch-ember")]
    GreaterEldritchEmber,

    [Description("Grand Eldritch Ember")]
    [TradeCurrencyType("grand-eldritch-ember")]
    GrandEldritchEmber,

    [Description("Exceptional Eldritch Ember")]
    [TradeCurrencyType("exceptional-eldritch-ember")]
    ExceptionalEldritchEmber,

    [Description("Lesser Eldritch Ichor")]
    [TradeCurrencyType("lesser-eldritch-ichor")]
    LesserEldritchIchor,

    [Description("Greater Eldritch Ichor")]
    [TradeCurrencyType("greater-eldritch-ichor")]
    GreaterEldritchIchor,

    [Description("Grand Eldritch Ichor")]
    [TradeCurrencyType("grand-eldritch-ichor")]
    GrandEldritchIchor,

    [Description("Exceptional Eldritch Ichor")]
    [TradeCurrencyType("exceptional-eldritch-ichor")]
    ExceptionalEldritchIchor,

    [Description("Orb of Conflict")]
    [TradeCurrencyType("orb-of-conflict")]
    OrbOfConflict,

    [Description("Tainted Chromatic Orb")]
    [TradeCurrencyType("tainted-chromatic-orb")]
    TaintedChromaticOrb,

    [Description("Tainted Orb of Fusing")]
    [TradeCurrencyType("tainted-orb-of-fusing")]
    TaintedOrbOfFusing,

    [Description("Tainted Jeweller's Orb")]
    [TradeCurrencyType("tainted-jewellers-orb")]
    TaintedJewellersOrb,

    [Description("Tainted Chaos Orb")]
    [TradeCurrencyType("tainted-chaos-orb")]
    TaintedChaosOrb,

    [Description("Tainted Exalted Orb")]
    [TradeCurrencyType("tainted-exalted-orb")]
    TaintedExaltedOrb,

    [Description("Tainted Mythic Orb")]
    [TradeCurrencyType("tainted-mythic-orb")]
    TaintedMythicOrb,

    [Description("Tainted Armourer's Scrap")]
    [TradeCurrencyType("tainted-armourers-scrap")]
    TaintedArmourersScrap,

    [Description("Tainted Blacksmith's Whetstone")]
    [TradeCurrencyType("tainted-blacksmiths-whetstone")]
    TaintedBlacksmithsWhetstone,

    [Description("Tainted Divine Teardrop")]
    [TradeCurrencyType("tainted-divine-teardrop")]
    TaintedDivineTeardrop,

    [Description("Wild Crystallised Lifeforce")]
    [TradeCurrencyType("wild-lifeforce")]
    WildLifeforce,

    [Description("Vivid Crystallised Lifeforce")]
    [TradeCurrencyType("vivid-lifeforce")]
    VividLifeforce,

    [Description("Primal Crystallised Lifeforce")]
    [TradeCurrencyType("primal-lifeforce")]
    PrimalLifeforce,

    [Description("Sacred Crystallised Lifeforce")]
    [TradeCurrencyType("sacred-lifeforce")]
    SacredLifeforce,

    [Description("Hinekora's Lock")]
    [TradeCurrencyType("hinekoras-lock")]
    HinekorasLock,

    [Description("Maven's Chisel of Procurement")]
    [TradeCurrencyType("mavens-chisel-of-procurement")]
    MavensChiselOfProcurement,

    [Description("Maven's Chisel of Proliferation")]
    [TradeCurrencyType("mavens-chisel-of-proliferation")]
    MavensChiselOfProliferation,

    [Description("Maven's Chisel of Divination")]
    [TradeCurrencyType("mavens-chisel-of-divination")]
    MavensChiselOfDivination,

    [Description("Maven's Chisel of Scarabs")]
    [TradeCurrencyType("mavens-chisel-of-scarabs")]
    MavensChiselOfScarabs,

    [Description("Maven's Chisel of Avarice")]
    [TradeCurrencyType("mavens-chisel-of-avarice")]
    MavensChiselOfAvarice,

    [Description("Reflecting Mist")]
    [TradeCurrencyType("reflecting-mist")]
    ReflectingMist,

    [Description("Chaos Shard")]
    [TradeCurrencyType("chaos-shard")]
    ChaosShard,

    [Description("Exalted Shard")]
    [TradeCurrencyType("exalted-shard")]
    ExaltedShard,

    [Description("Engineer's Shard")]
    [TradeCurrencyType("engineers-shard")]
    EngineersShard,

    [Description("Regal Shard")]
    [TradeCurrencyType("regal-shard")]
    RegalShard,

    [Description("Annulment Shard")]
    [TradeCurrencyType("annulment-shard")]
    AnnulmentShard,

    [Description("Binding Shard")]
    [TradeCurrencyType("binding-shard")]
    BindingShard,

    [Description("Ancient Shard")]
    [TradeCurrencyType("ancient-shard")]
    AncientShard,

    [Description("Horizon Shard")]
    [TradeCurrencyType("horizon-shard")]
    HorizonShard,

    [Description("Harbinger's Shard")]
    [TradeCurrencyType("harbingers-shard")]
    HarbingersShard,

    [Description("Fracturing Shard")]
    [TradeCurrencyType("fracturing-shard")]
    FracturingShard,

    [Description("Mirror Shard")]
    [TradeCurrencyType("mirror-shard")]
    MirrorShard,

    [Description("Transmutation Shard")]
    [TradeCurrencyType("transmutation-shard")]
    TransmutationShard,

    [Description("Chance Shard")]
    [TradeCurrencyType("chance-shard")]
    ChanceShard,

    [Description("Artificer's Shard")]
    [TradeCurrencyType("artificers-shard")]
    ArtificersShard,

    [Description("Artificer's Orb")]
    [TradeCurrencyType("artificers")]
    ArtificersOrb,

    [Description("Lesser Jeweller's Orb")]
    [TradeCurrencyType("lesser-jewellers-orb")]
    LesserJewellersOrb,

    [Description("Greater Jeweller's Orb")]
    [TradeCurrencyType("greater-jewellers-orb")]
    GreaterJewellersOrb,

    [Description("Perfect Jeweller's Orb")]
    [TradeCurrencyType("perfect-jewellers-orb")]
    PerfectJewellersOrb,

    [Description("Arcanist's Etcher")]
    [TradeCurrencyType("etcher")]
    ArcanistsEtcher
}