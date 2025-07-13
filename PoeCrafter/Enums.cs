using System.ComponentModel;

namespace PoeCrafter;

public enum AffixTier
{
    Tier1,
    Tier2,
    Tier3,
    Tier4,
    Tier5,
    Tier6,
    Tier7,
    Tier8,
    Tier9,
    Unknown
}

public enum AffixType
{
    Prefix,
    Suffix,
    Unknown
}

public enum ModType
{
    [Description("Adds # to # Physical Damage")]
    [AffixPattern(@"Adds # to # Physical Damage")]
    FlatPhysical,
    [Description("#% increased Physical Damage")]
    [AffixPattern(@"#% increased Physical Damage")]
    PercentPhysical,
    [Description("#% increased Physical Damage / +# to Accuracy Rating")]
    [AffixPattern(@"#% increased Physical Damage")]
    [AffixPattern(@"\+# to Accuracy Rating")]
    HybridPhysical,
    [Description("+# to Level of Socketed Gems")]
    [AffixPattern(@"\+# to Level of Socketed Gems")]
    LevelSocketedGems,
    [Description("+# to Level of Socketed Melee Gems")]
    [AffixPattern(@"\+# to Level of Socketed Melee Gems")]
    LevelSocketedMeleeGems,
    [Description("#% of Physical Attack Damage Leeched as Life")]
    [AffixPattern(@"#% of Physical Attack Damage Leeched as Life")]
    LifeLeech,
    [Description("#% of Physical Attack Damage Leeched as Mana")]
    [AffixPattern(@"#% of Physical Attack Damage Leeched as Mana")]
    ManaLeech,
    [Description("#% increased Elemental Damage with Attack Skills")]
    [AffixPattern(@"#% increased Elemental Damage with Attack Skills")]
    ElementalDamageWithAttacks,
    [Description("Adds # to # Cold Damage")]
    [AffixPattern(@"Adds # to # Cold Damage")]
    FlatCold,
    [Description("Adds # to # Fire Damage")]
    [AffixPattern(@"Adds # to # Fire Damage")]
    FlatFire,
    [Description("Adds # to # Lightning Damage")]
    [AffixPattern(@"Adds # to # Lightning Damage")]
    FlatLightning,
    [Description("Adds # to # Chaos Damage")]
    [AffixPattern(@"Adds # to # Chaos Damage")]
    FlatChaos,
    [Description("+# to Accuracy Rating")]
    [AffixPattern(@"\+# to Accuracy Rating")]
    FlatAccuracy,
    [Description("#% increased Light Radius / +# or #% to Accuracy Rating")]
    [AffixPattern(@"#% increased Light Radius")]
    [AffixPattern(@"(\+# to)?(#% increased)? Accuracy Rating")]
    LightRadiusAndAccuracy,
    [Description("#% increased Attack Speed")]
    [AffixPattern(@"#% increased Attack Speed")]
    AttackSpeed,
    [Description("+# to Dexterity")]
    [AffixPattern(@"\+# to Dexterity")]
    Dexterity,
    [Description("+# to Strength")]
    [AffixPattern(@"\+# to Strength")]
    Strength,
    [Description("#% increased Critical Strike Chance")]
    [AffixPattern(@"#% increased Critical Strike Chance")]
    CritChance,
    [Description("+#% to Global Critical Strike Multiplier")]
    [AffixPattern(@"\+#% to Global Critical Strike Multiplier")]
    CritMultiplier,
    [Description("+# Life gained on Kill")]
    [AffixPattern(@"\+# Life gained on Kill")]
    LifeOnKill,
    [Description("+# Life gained for each Enemy hit by Attacks")]
    [AffixPattern(@"\+# Life gained for each Enemy hit by Attacks")]
    LifeOnHit,
    [Description("+# Mana gained on Kill")]
    [AffixPattern(@"\+# Mana gained on Kill")]
    ManaOnKill,
    [Description("+#% to Cold Resistance")]
    [AffixPattern(@"\+#% to Cold Resistance")]
    ColdResistance,
    [Description("+#% to Fire Resistance")]
    [AffixPattern(@"\+#% to Fire Resistance")]
    FireResistance,
    [Description("+#% to Lightning Resistance")]
    [AffixPattern(@"\+#% to Lightning Resistance")]
    LightningResistance,
    [Description("+#% to Chaos Resistance")]
    [AffixPattern(@"\+#% to Chaos Resistance")]
    ChaosResistance,
    [Description("#% reduced Attribute Requirements")]
    [AffixPattern(@"#% reduced Attribute Requirements")]
    ReducedAttributeReq,
    [Description("#% increased Stun Duration on Enemies")]
    [AffixPattern(@"#% increased Stun Duration on Enemies")]
    StunDuration,
    [Description("#% reduced Enemy Stun Threshold")]
    [AffixPattern(@"#% reduced Enemy Stun Threshold")]
    StunThreshold,
    [Description("#% increased Damage with Poison")]
    [AffixPattern(@"#% increased Damage with Poison")]
    DamageWithPoison,
    [Description("#% chance to Poison on Hit")]
    [AffixPattern(@"#% chance to Poison on Hit")]
    ChanceToPoison,
    [Description("#% increased Poison Duration")]
    [AffixPattern(@"#% increased Poison Duration")]
    PoisonDuration,
    [Description("#% increased Damage with Bleeding")]
    [AffixPattern(@"#% increased Damage with Bleeding")]
    DamageWithBleeding,
    [Description("#% chance to cause Bleeding on Hit")]
    [AffixPattern(@"#% chance to cause Bleeding on Hit")]
    ChanceToBleed,
    [Description("#% increased Bleeding Duration")]
    [AffixPattern(@"#% increased Bleeding Duration")]
    BleedDuration,
    [Description("Unknown Modifier")]
    Unknown
}
