using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PoeLib;

public enum Strictness : int
{
    Chaos7 = 7,
    Chaos15 = 15,
    Chaos30 = 30,
    Div0p5 = 50,
    Div0p75 = 75,
    Div1 = 100,
    Div1p5 = 150,
    Div2 = 200,
    Div4 = 400
}

public enum ServiceLocation
{
    Local,
    Remote,
}

public enum Indexer
{
    None,
    Public
}

public enum PoeItemRarity
{
    normal,
    magic,
    rare,
    unique,
    uniquefoil,
    nonunique,
    any
}

public enum CurrencyExchangeType
{
    Buy,
    Sell
}

public enum MessageSource
{
    Player,
    Me
}

public enum Rarity
{
    Normal,
    Magic,
    Rare,
    Unique,
    Gem,
    Currency,
    DivinationCard,
    QuestItem,
    Prophecy,
    UniqueFoil,
    Any
}

public enum SocketColor
{
    B,
    G,
    R,
    W,
    DV,
    A
}

public enum MonitorResolution
{
    Res_Unknown,
    Res_1280x1024,
    Res_1920x1080,
    Res_2560x1440
}

public enum CurrencyType
{
    [Description("None")]
    [StackSize(0)]
    none,
    [Description("Orb of Alteration")]
    [StackSize(20)]
    [TradeName("alteration")]
    alt,
    [Description("Alteration Shard")]
    [StackSize(20)]
    altshard,
    [Description("Orb of Fusing")]
    [StackSize(20)]
    [TradeName("fusing")]
    fusing,
    [Description("Orb of Alchemy")]
    [StackSize(20)]
    [TradeName("alchemy")]
    alch,
    [Description("Alchemy Shard")]
    [StackSize(10)]
    alchshard,
    [Description("Chaos Orb")]
    [StackSize(20)]
    chaos,
    [Description("Gemcutter's Prism")]
    [StackSize(20)]
    gcp,
    [Description("Exalted Orb")]
    [StackSize(20)]
    [TradeName("exalted")]
    exalted,
    [Description("Chromatic Orb")]
    [StackSize(20)]
    chrome,
    [Description("Jeweller's Orb")]
    [StackSize(20)]
    [TradeName("jewellers")]
    jewellers,
    [Description("Engineer's Orb")]
    [StackSize(20)]
    engineers,
    [Description("Infused Engineer's Orb")]
    [TradeName("infused-engineers-orb")]
    [StackSize(20)]
    infusedengineers,
    [Description("Orb of Chance")]
    [StackSize(20)]
    chance,
    [Description("Cartographer's Chisel")]
    [StackSize(20)]
    chisel,
    [Description("Orb of Scouring")]
    [StackSize(30)]
    scour,
    [Description("Blessed Orb")]
    [StackSize(20)]
    blessed,
    [Description("Orb of Regret")]
    [StackSize(40)]
    regret,
    [Description("Regal Orb")]
    [StackSize(20)]
    regal,
    [Description("Divine Orb")]
    [StackSize(20)]
    divine,
    [Description("Vaal Orb")]
    [StackSize(20)]
    vaal,
    [Description("Orb of Annulment")]
    [StackSize(20)]
    annul,
    [Description("Orb of Binding")]
    [TradeName("orb-of-binding")]
    [StackSize(20)]
    orbofbinding,
    [Description("Ancient Orb")]
    [TradeName("ancient-orb")]
    [StackSize(20)]
    ancientorb,
    [Description("Orb of Horizons")]
    [TradeName("orb-of-horizons")]
    [StackSize(20)]
    orbofhorizons,
    [Description("Harbinger's Orb")]
    [TradeName("harbingers-orb")]
    [StackSize(20)]
    harbingersorb,
    [Description("Scroll Fragment")]
    [StackSize(40)]
    wisdomfragment,
    [Description("Scroll of Wisdom")]
    [StackSize(40)]
    wisdom,
    [Description("Portal Scroll")]
    [StackSize(40)]
    portal,
    [Description("Armourer's Scrap")]
    [StackSize(40)]
    scrap,
    [Description("Blacksmith's Whetstone")]
    [StackSize(20)]
    whetstone,
    [Description("Glassblower's Bauble")]
    [StackSize(20)]
    bauble,
    [Description("Orb of Transmutation")]
    [StackSize(40)]
    transmute,
    [Description("Transmutation Shard")]
    [StackSize(40)]
    transmuteshard,
    [Description("Orb of Augmentation")]
    [StackSize(30)]
    aug,
    [Description("Mirror of Kalandra")]
    [StackSize(10)]
    mirror,
    [Description("Eternal Orb")]
    [StackSize(10)]
    eternal,
    [Description("Perandus Coin")]
    [StackSize(1000)]
    [TradeName("coin")]
    p,
    [Description("Rogue's Marker")]
    [TradeName("rogues-marker")]
    [StackSize(30)]
    roguesmarker,
    [Description("Silver Coin")]
    [StackSize(30)]
    silver,
    [Description("Crusader's Exalted Orb")]
    [TradeName("crusaders-exalted-orb")]
    [StackSize(10)]
    crudsadersexaltedorb,
    [Description("Redeemer's Exalted Orb")]
    [TradeName("redeemers-exalted-orb")]
    [StackSize(10)]
    redeemersexaltedorb,
    [Description("Hunter's Exalted Orb")]
    [TradeName("hunters-exalted-orb")]
    [StackSize(10)]
    huntersexaltedorb,
    [Description("Warlord's Exalted Orb")]
    [TradeName("warlords-exalted-orb")]
    [StackSize(10)]
    warlordsexaltedorb,
    [Description("Awakener's Orb")]
    [TradeName("awakeners-orb")]
    [StackSize(10)]
    awakenersorb,
    [Description("Orb of Dominance")]
    [TradeName("orb-of-dominance")]
    [StackSize(10)]
    orbofdominance,
    [Description("Facetor's Lens")]
    [TradeName("facetors")]
    [StackSize(10)]
    facetors,
    [Description("Prime Regrading Lens")]
    [TradeName("prime-regrading-lens")]
    [StackSize(10)]
    primeregrading,
    [Description("Secondary Regrading Lens")]
    [TradeName("secondary-regrading-lens")]
    [StackSize(10)]
    secondaryregrading,
    [Description("Tailoring Orb")]
    [TradeName("tailoring-orb")]
    [StackSize(10)]
    tailoringorb,
    [Description("Tempering Orb")]
    [TradeName("tempering-orb")]
    [StackSize(10)]
    temperingorb,
    [Description("Stacked Deck")]
    [TradeName("stacked-deck")]
    [StackSize(10)]
    stackeddeck,
    [Description("Ritual Vessel")]
    [TradeName("ritual-vessel")]
    [StackSize(10)]
    ritualvessel,
    [Description("Awakened Sextant")]
    [TradeName("master-sextant")]
    [StackSize(10)]
    awakenedsextant,
    [Description("Elevated Sextant")]
    [TradeName("elevated-sextant")]
    [StackSize(10)]
    elevatedsextant,
    [Description("Surveyor's Compass")]
    [TradeName("surveyors-compass")]
    [StackSize(10)]
    surveyorscompass,
    [Description("Charged Compass")]
    [StackSize(10)]
    chargedcompass,
    [Description("Orb of Unmaking")]
    [TradeName("orb-of-unmaking")]
    [StackSize(10)]
    unmaking,
    [Description("Blessing of Xoph")]
    [TradeName("blessing-xoph")]
    [StackSize(10)]
    blessingxoph,
    [Description("Blessing of Tul")]
    [TradeName("blessing-tul")]
    [StackSize(10)]
    blessingtul,
    [Description("Blessing of Esh")]
    [TradeName("blessing-esh")]
    [StackSize(10)]
    blessingesh,
    [Description("Blessing of Uul-Netol")]
    [TradeName("blessing-uul-netol")]
    [StackSize(10)]
    blessinguulnetol,
    [Description("Blessing of Chayula")]
    [TradeName("blessing-chayula")]
    [StackSize(10)]
    blessingchayula,
    [Description("Veiled Chaos Orb")]
    [TradeName("veiled-chaos-orb")]
    [StackSize(10)]
    veiledchaos,
    [Description("Annulment Shard")]
    [TradeName("annulment-shard")]
    [StackSize(20)]
    annulmentshard,
    [Description("Mirror Shard")]
    [TradeName("mirror-shard")]
    [StackSize(20)]
    mirrorshard,
    [Description("Exalted Shard")]
    [StackSize(20)]
    [TradeName("exalted-shard")]
    exaltedshard,
    [Description("Binding Shard")]
    [TradeName("binding-shard")]
    [StackSize(20)]
    bindingshard,
    [Description("Horizon Shard")]
    [TradeName("horizon-shard")]
    [StackSize(20)]
    horizonshard,
    [Description("Harbinger's Shard")]
    [TradeName("harbingers-shard")]
    [StackSize(20)]
    harbingersshard,
    [Description("Engineer's Shard")]
    [TradeName("engineers-shard")]
    [StackSize(20)]
    engineersshard,
    [Description("Ancient Shard")]
    [TradeName("ancient-shard")]
    [StackSize(20)]
    ancientshard,
    [Description("Chaos Shard")]
    [TradeName("chaos-shard")]
    [StackSize(20)]
    chaosshard,
    [Description("Regal Shard")]
    [TradeName("regal-shard")]
    [StackSize(20)]
    regalshard,
    [Description("Splinter of Xoph")]
    [TradeName("splinter-xoph")]
    [StackSize(100)]
    splinterxoph,
    [Description("Splinter of Tul")]
    [TradeName("splinter-tul")]
    [StackSize(100)]
    splintertul,
    [Description("Splinter of Esh")]
    [TradeName("splinter-esh")]
    [StackSize(100)]
    splinteresh,
    [Description("Splinter of Uul-Netol")]
    [TradeName("splinter-uul")]
    [StackSize(100)]
    splinteruulnetol,
    [Description("Splinter of Chayula")]
    [TradeName("splinter-chayula")]
    [StackSize(100)]
    splinterchayula,
    [Description("Timeless Karui Splinter")]
    [TradeName("timeless-karui-splinter")]
    [StackSize(100)]
    timelesskaruisplinter,
    [Description("Timeless Maraketh Splinter")]
    [TradeName("timeless-maraketh-splinter")]
    [StackSize(100)]
    timelessmarakethsplinter,
    [Description("Timeless Eternal Empire Splinter")]
    [TradeName("timeless-eternal-empire-splinter")]
    [StackSize(100)]
    timelesseternalempiresplinter,
    [Description("Timeless Templar Splinter")]
    [TradeName("timeless-templar-splinter")]
    [StackSize(100)]
    timelesstemplarsplinter,
    [Description("Timeless Vaal Splinter")]
    [TradeName("timeless-vaal-splinter")]
    [StackSize(100)]
    timelessvaalsplinter,
    [Description("Simulacrum Splinter")]
    [TradeName("simulacrum-splinter")]
    [StackSize(300)]
    simulacrumsplinter,
    [Description("Crescent Splinter")]
    [TradeName("crescent-splinter")]
    [StackSize(10)]
    crescentsplinter,
    [Description("Ritual Splinter")]
    [TradeName("ritual-splinter")]
    [StackSize(100)]
    ritualsplinter,
    [Description("Sacrifice at Dusk")]
    [StackSize(1)]
    dusk,
    [Description("Sacrifice at Midnight")]
    [StackSize(1)]
    mid,
    [Description("Sacrifice at Dawn")]
    [StackSize(1)]
    dawn,
    [Description("Sacrifice at Noon")]
    [StackSize(1)]
    noon,
    [Description("Mortal Grief")]
    [StackSize(1)]
    grief,
    [Description("Mortal Rage")]
    [StackSize(1)]
    rage,
    [Description("Mortal Hope")]
    [StackSize(1)]
    hope,
    [Description("Mortal Ignorance")]
    [StackSize(1)]
    ign,
    [Description("Eber's Key")]
    [StackSize(1)]
    eber,
    [Description("Yriel's Key")]
    [StackSize(1)]
    yriel,
    [Description("Inya's Key")]
    [StackSize(1)]
    inya,
    [Description("Volkuur's Key")]
    [StackSize(1)]
    volkuur,
    [Description("Fragment of the Hydra")]
    [StackSize(1)]
    hydra,
    [Description("Fragment of the Phoenix")]
    [StackSize(1)]
    phoenix,
    [Description("Fragment of the Minotaur")]
    [StackSize(1)]
    minot,
    [Description("Fragment of the Chimera")]
    [StackSize(1)]
    chimer,
    [Description("Fragment of Enslavement")]
    [TradeName("fragment-of-enslavement")]
    [StackSize(1)]
    enslavement,
    [Description("Fragment of Eradication")]
    [TradeName("fragment-of-eradication")]
    [StackSize(1)]
    eradication,
    [Description("Fragment of Constriction")]
    [TradeName("fragment-of-constriction")]
    [StackSize(1)]
    constriction,
    [Description("Fragment of Purification")]
    [TradeName("fragment-of-purification")]
    [StackSize(1)]
    purification,
    [Description("Fragment of Terror")]
    [TradeName("fragment-of-terror")]
    [StackSize(1)]
    terror,
    [Description("Fragment of Emptiness")]
    [TradeName("fragment-of-emptiness")]
    [StackSize(1)]
    emptiness,
    [Description("Fragment of Shape")]
    [TradeName("fragment-of-shape")]
    [StackSize(1)]
    shape,
    [Description("Fragment of Knowledge")]
    [TradeName("fragment-of-knowledge")]
    [StackSize(1)]
    knowledge,
    [Description("Key to Decay")]
    [TradeName("key-to-decay")]
    [StackSize(1)]
    decay,
    [Description("Maddening Object")]
    [TradeName("maddening-object")]
    [StackSize(1)]
    maddeningobject,
    [Description("Xoph's Breachstone")]
    [TradeName("xophs-breachstone")]
    [StackSize(1)]
    xophsbreachstone,
    [Description("Tul's Breachstone")]
    [TradeName("tuls-breachstone")]
    [StackSize(1)]
    tulsbreachstone,
    [Description("Esh's Breachstone")]
    [TradeName("eshs-breachstone")]
    [StackSize(1)]
    eshsbreachstone,
    [Description("Uul-Nethol's Breachstone")]
    [TradeName("uul-breachstone")]
    [StackSize(1)]
    uulbreachstone,
    [Description("Chayula's Breachstone")]
    [TradeName("chayulas-breachstone")]
    [StackSize(1)]
    chayulasbreachstone,
    [Description("Xoph's Charged Breachstone")]
    [TradeName("xophs-charged-breachstone")]
    [StackSize(1)]
    xophschargedbreachstone,
    [Description("Tul's Charged Breachstone")]
    [TradeName("tuls-charged-breachstone")]
    [StackSize(1)]
    tulschargedbreachstone,
    [Description("Esh's Charged Breachstone")]
    [TradeName("eshs-charged-breachstone")]
    [StackSize(1)]
    eshschargedbreachstone,
    [Description("Uul-Nethol's Charged Breachstone")]
    [TradeName("uul-charged-breachstone")]
    [StackSize(1)]
    uulchargedbreachstone,
    [Description("Chayula's Charged Breachstone")]
    [TradeName("chayulas-charged-breachstone")]
    [StackSize(1)]
    chayulaschargedbreachstone,
    [Description("Xoph's Enriched Breachstone")]
    [TradeName("xophs-enriched-breachstone")]
    [StackSize(1)]
    xophsenrichedbreachstone,
    [Description("Tul's Enriched Breachstone")]
    [TradeName("tuls-enriched-breachstone")]
    [StackSize(1)]
    tulsenrichedbreachstone,
    [Description("Esh's Enriched Breachstone")]
    [TradeName("eshs-enriched-breachstone")]
    [StackSize(1)]
    eshsenrichedbreachstone,
    [Description("Uul-Nethol's Enriched Breachstone")]
    [TradeName("uul-enriched-breachstone")]
    [StackSize(1)]
    uulenrichedbreachstone,
    [Description("Chayula's Enriched Breachstone")]
    [TradeName("chayulas-enriched-breachstone")]
    [StackSize(1)]
    chayulasenrichedbreachstone,
    [Description("Xoph's Pure Breachstone")]
    [TradeName("xophs-pure-breachstone")]
    [StackSize(1)]
    xophspurebreachstone,
    [Description("Tul's Pure Breachstone")]
    [TradeName("tuls-pure-breachstone")]
    [StackSize(1)]
    tulspurebreachstone,
    [Description("Esh's Pure Breachstone")]
    [TradeName("eshs-pure-breachstone")]
    [StackSize(1)]
    eshspurebreachstone,
    [Description("Uul-Nethol's Pure Breachstone")]
    [TradeName("uul-pure-breachstone")]
    [StackSize(1)]
    uulpurebreachstone,
    [Description("Chayula's Pure Breachstone")]
    [TradeName("chayulas-pure-breachstone")]
    [StackSize(1)]
    chayulaspurebreachstone,
    [Description("Timeless Karui Emblem")]
    [TradeName("timeless-karui-splinter")]
    [StackSize(1)]
    timelesskaruiemblem,
    [Description("Timeless Maraketh Emblem")]
    [TradeName("timeless-maraketh-emblem")]
    [StackSize(1)]
    timelessmarakethemblem,
    [Description("Timeless Eternal Empire Emblem")]
    [TradeName("timeless-eternal-empire-emblem")]
    [StackSize(1)]
    timelesseternalempireemblem,
    [Description("Timeless Templar Emblem")]
    [TradeName("timeless-templar-emblem")]
    [StackSize(1)]
    timelesstemplaremblem,
    [Description("Timeless Vaal Emblem")]
    [TradeName("timeless-vaal-emblem")]
    [StackSize(1)]
    timelessvaalemblem,
    [Description("Simulacrum")]
    [StackSize(1)]
    simulacrum,
    [Description("Offering to the Goddess")]
    [StackSize(1)]
    offer,
    [Description("Tribute to the Goddess")]
    [TradeName("offer-tribute")]
    [StackSize(1)]
    offertribute,
    [Description("Gift to the Goddess")]
    [TradeName("offer-gift")]
    [StackSize(1)]
    offergift,
    [Description("Dedication to the Goddess")]
    [TradeName("offer-dedication")]
    [StackSize(1)]
    offerdedication,
    [Description("The Maven's Writ")]
    [TradeName("the-mavens-writ")]
    [StackSize(1)]
    mavenswrit,
    [Description("Ancient Reliquary Key")]
    [TradeName("ancient-reliquary-key")]
    [StackSize(1)]
    ancientreliquarykey,
    [Description("Timeworn Reliquary Key")]
    [TradeName("timeworn-reliquary-key")]
    [StackSize(1)]
    timewornreliquarykey,
    [Description("Divine Vessel")]
    [TradeName("divine-vessel")]
    [StackSize(1)]
    divinevessel,
    [Description("Imprint")]
    [StackSize(1)]
    imprint,
    [Description("Turbulent Catalyst")]
    [StackSize(10)]
    turbulentcatalyst,
    [Description("Imbued Catalyst")]
    [StackSize(10)]
    imbuedcatalyst,
    [Description("Abrasive Catalyst")]
    [StackSize(10)]
    abrasivecatalyst,
    [Description("Tempering Catalyst")]
    [StackSize(10)]
    temperingcatalyst,
    [Description("Fertile Catalyst")]
    [StackSize(10)]
    fertilecatalyst,
    [Description("Prismatic Catalyst")]
    [StackSize(10)]
    prismaticcatalyst,
    [Description("Intrinsic Catalyst")]
    [StackSize(10)]
    intrinsiccatalyst,
    [Description("Clear Oil")]
    [StackSize(10)]
    clearoil,
    [Description("Sepia Oil")]
    [StackSize(10)]
    sepiaoil,
    [Description("Amber Oil")]
    [StackSize(10)]
    amberoil,
    [Description("Verdant Oil")]
    [StackSize(10)]
    verdantoil,
    [Description("Teal Oil")]
    [StackSize(10)]
    tealoil,
    [Description("Azure Oil")]
    [StackSize(10)]
    azureoil,
    [Description("Indigo Oil")]
    [StackSize(10)]
    indigooil,
    [Description("Violet Oil")]
    [StackSize(10)]
    violetoil,
    [Description("Crimson Oil")]
    [StackSize(10)]
    crimsonoil,
    [Description("Black Oil")]
    [StackSize(10)]
    blackoil,
    [Description("Opalescent Oil")]
    [StackSize(10)]
    opalescentoil,
    [Description("Silver Oil")]
    [StackSize(10)]
    silveroil,
    [Description("Golden Oil")]
    [StackSize(10)]
    goldenoil,
    [Description("Enkindling Orb")]
    [TradeName("enkindling-orb")]
    [StackSize(10)]
    enkindling,
    [Description("Instilling Orb")]
    [TradeName("instilling-orb")]
    [StackSize(10)]
    instilling,
    [Description("Sacred Orb")]
    [TradeName("sacred-orb")]
    [StackSize(10)]
    sacred,
    [Description("Orb of Conflict")]
    [StackSize(10)]
    orbofconflict,
    [Description("Tainted Divine Teardrop")]
    [StackSize(20)]
    tainteddivineteardrop,
    [Description("Tainted Chromatic Orb")]
    [StackSize(20)]
    taintedchrome,
    [Description("Tained Orb of Fusing")]
    [StackSize(20)]
    taintedfuse,
    [Description("Tained Jeweller's Orb")]
    [StackSize(20)]
    taintedjewellers,
    [Description("Tained Chaos Orb")]
    [StackSize(10)]
    taintedchaos,
    [Description("Tained Exalted Orb")]
    [StackSize(10)]
    taintedexa,
    [Description("Tained Mythic Orb")]
    [StackSize(20)]
    taintedmythic,
    [Description("Tained Armourer's Scrap")]
    [StackSize(20)]
    taintedscrap,
    [Description("Tained Blacksmith's Whetstone")]
    [StackSize(20)]
    taintedwhetstone,
    [Description("Tained Blessing")]
    [StackSize(20)]
    taintedblessing,
    [Description("Eldritch Chaos Orb")]
    [StackSize(10)]
    eldritchchaosorb,
    [Description("Eldritch Exalted Orb")]
    [StackSize(10)]
    eldritchexaltedorb,
    [Description("Eldritch Annulment Orb")]
    [StackSize(10)]
    eldritchannulmentorb,
    [Description("Lesser Eldritch Ember")]
    [StackSize(10)]
    lessereldritchember,
    [Description("Greater Eldritch Ember")]
    [StackSize(10)]
    greatereldritchember,
    [Description("Grand Eldritch Ember")]
    [StackSize(10)]
    grandeldritchember,
    [Description("Exceptional Eldritch Ember")]
    [StackSize(10)]
    exceptionaleldritchember,
    [Description("Lesser Eldritch Ichor")]
    [StackSize(10)]
    lessereldritchichor,
    [Description("Greater Eldritch Ichor")]
    [StackSize(10)]
    greatereldritchichor,
    [Description("Grand Eldritch Ichor")]
    [StackSize(10)]
    grandeldritchichor,
    [Description("Exceptional Eldritch Ichor")]
    [StackSize(10)]
    exceptionaleldritchichor,
    [Description("Wild Crystallised Lifeforce")]
    [StackSize(10)]
    wildcrystalizedlifeforce,
    [Description("Vivid Crystallised Lifeforce")]
    [StackSize(10)]
    vividcrystallisedlifeforce,
    [Description("Primal Crystallised Lifeforce")]
    [StackSize(10)]
    primalcrystallisedlifeforce,
    [Description("Sacred Crystallised Lifeforce")]
    [StackSize(10)]
    sacredcrystallisedlifeforce,
    [Description("Singular Scouting Report")]
    [StackSize(20)]
    singularreport,
    [Description("Otherworldly Scouting Report")]
    [StackSize(20)]
    otherworldlyreport,
    [Description("Comprehensive Scouting Report")]
    [StackSize(20)]
    comprehensivereport,
    [Description("Vaal Scouting Report")]
    [StackSize(20)]
    vaalreport,
    [Description("Delirious Scouting Report")]
    [StackSize(20)]
    deliriousreport,
    [Description("Operative's Scouting Report")]
    [StackSize(20)]
    operativesreport,
    [Description("Blighted Scouting Report")]
    [StackSize(20)]
    blightedreport,
    [Description("Influenced Scouting Report")]
    [StackSize(20)]
    influencedreport,
    [Description("Explorer's Scouting Report")]
    [StackSize(20)]
    explorersreport,
    [Description("Fracturing Orb")]
    [StackSize(20)]
    fracturingorb,
    [Description("Fracturing Shard")]
    [StackSize(20)]
    fracturingshard,
    [Description("Deafening Essence of Scorn")]
    [StackSize(9)]
    deafeningessenceofscorn
}

public enum State
{
    Town,
    Hideout,
    CharacterInfoOpen,
    PartyInviteSent,
    HomeWithPlayerAway,
    HomeWithPlayerInOwnHideout,
    AwayInPlayersHideout,
    BothInPlayersHideout,
    TradeInviteSent,
    TradeOpened,
    TradeAccepted
}

public enum Trigger
{
    GotoHideout,
    GotoTown,
    GotoPartyHideout,
    InviteToParty,
    PlayerJoinedParty,
    LeaveParty,
    KickPlayer,
    PlayerJoinedHideout,
    PlayerHideoutJoinTimeout,
    TradeInviteAccepted,
    ItemTradeInviteTimeout,
    CurrencyTradeInviteTimeout,
    InitiateTrade,
    AcceptTrade,
    CompleteCurrencyTrade,
    CompleteItemTrade,
    CancelCurrencyTrade,
    PlayerJoinAreaTimeout,
    JoinParty,
    CancelItemTrade
}

public static class EnumHelper
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }
}
