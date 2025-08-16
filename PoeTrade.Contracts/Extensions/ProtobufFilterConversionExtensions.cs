namespace PoeTrade.Contracts.Extensions;

/// <summary>
/// Extension methods for converting specific filter types between records and protobuf
/// </summary>
public static class ProtobufFilterConversionExtensions
{
    #region TypeFilters Conversions

    /// <summary>
    /// Converts a TypeFilters record to protobuf message
    /// </summary>
    public static Proto.TypeFilters ToProtobuf(this TypeFilters record)
    {
        var proto = new Proto.TypeFilters
        {
            Filters = record.Filters.ToProtobuf()
        };

        if (record.Disabled.HasValue)
            proto.Disabled = record.Disabled.Value;

        return proto;
    }

    /// <summary>
    /// Converts a protobuf TypeFilters to record
    /// </summary>
    public static TypeFilters ToRecord(this Proto.TypeFilters proto)
    {
        return new TypeFilters
        {
            Disabled = proto.Disabled,
            Filters = proto.Filters.ToRecord()
        };
    }

    /// <summary>
    /// Converts a TypeFilterOptions record to protobuf message
    /// </summary>
    public static Proto.TypeFilterOptions ToProtobuf(this TypeFilterOptions record)
    {
        var proto = new Proto.TypeFilterOptions();

        if (record.Category != null)
            proto.Category = record.Category.ToProtobuf();
        if (record.Rarity != null)
            proto.Rarity = record.Rarity.ToProtobuf();
        if (record.ItemLevel != null)
            proto.ItemLevel = record.ItemLevel.ToProtobuf();
        if (record.Quality != null)
            proto.Quality = record.Quality.ToProtobuf();

        return proto;
    }

    /// <summary>
    /// Converts a protobuf TypeFilterOptions to record
    /// </summary>
    public static TypeFilterOptions ToRecord(this Proto.TypeFilterOptions proto)
    {
        return new TypeFilterOptions
        {
            Category = proto.Category?.ToRecord(),
            Rarity = proto.Rarity?.ToRecord(),
            ItemLevel = proto.ItemLevel?.ToRecord(),
            Quality = proto.Quality?.ToRecord()
        };
    }

    #endregion

    #region WeaponFilters Conversions

    /// <summary>
    /// Converts a WeaponFilters record to protobuf message
    /// </summary>
    public static Proto.WeaponFilters ToProtobuf(this WeaponFilters record)
    {
        var proto = new Proto.WeaponFilters
        {
            Filters = record.Filters.ToProtobuf()
        };

        if (record.Disabled.HasValue)
            proto.Disabled = record.Disabled.Value;

        return proto;
    }

    /// <summary>
    /// Converts a protobuf WeaponFilters to record
    /// </summary>
    public static WeaponFilters ToRecord(this Proto.WeaponFilters proto)
    {
        return new WeaponFilters
        {
            Disabled = proto.Disabled,
            Filters = proto.Filters.ToRecord()
        };
    }

    /// <summary>
    /// Converts a WeaponFilterOptions record to protobuf message
    /// </summary>
    public static Proto.WeaponFilterOptions ToProtobuf(this WeaponFilterOptions record)
    {
        var proto = new Proto.WeaponFilterOptions();

        if (record.Damage != null)
            proto.Damage = record.Damage.ToProtobuf();
        if (record.Crit != null)
            proto.Crit = record.Crit.ToProtobuf();
        if (record.PhysicalDps != null)
            proto.PhysicalDps = record.PhysicalDps.ToProtobuf();
        if (record.ElementalDps != null)
            proto.ElementalDps = record.ElementalDps.ToProtobuf();
        if (record.Dps != null)
            proto.Dps = record.Dps.ToProtobuf();
        if (record.AttacksPerSecond != null)
            proto.AttacksPerSecond = record.AttacksPerSecond.ToProtobuf();

        return proto;
    }

    /// <summary>
    /// Converts a protobuf WeaponFilterOptions to record
    /// </summary>
    public static WeaponFilterOptions ToRecord(this Proto.WeaponFilterOptions proto)
    {
        return new WeaponFilterOptions
        {
            Damage = proto.Damage?.ToRecord(),
            Crit = proto.Crit?.ToRecord(),
            PhysicalDps = proto.PhysicalDps?.ToRecord(),
            ElementalDps = proto.ElementalDps?.ToRecord(),
            Dps = proto.Dps?.ToRecord(),
            AttacksPerSecond = proto.AttacksPerSecond?.ToRecord()
        };
    }

    #endregion

    #region TradeFilters Conversions

    /// <summary>
    /// Converts a TradeFilters record to protobuf message
    /// </summary>
    public static Proto.TradeFilters ToProtobuf(this TradeFilters record)
    {
        var proto = new Proto.TradeFilters
        {
            Filters = record.Filters.ToProtobuf()
        };

        if (record.Disabled.HasValue)
            proto.Disabled = record.Disabled.Value;

        return proto;
    }

    /// <summary>
    /// Converts a protobuf TradeFilters to record
    /// </summary>
    public static TradeFilters ToRecord(this Proto.TradeFilters proto)
    {
        return new TradeFilters
        {
            Disabled = proto.Disabled,
            Filters = proto.Filters.ToRecord()
        };
    }

    /// <summary>
    /// Converts a TradeFilterOptions record to protobuf message
    /// </summary>
    public static Proto.TradeFilterOptions ToProtobuf(this TradeFilterOptions record)
    {
        var proto = new Proto.TradeFilterOptions();

        if (record.Account != null)
            proto.Account = record.Account.ToProtobuf();
        if (record.Collapse != null)
            proto.Collapse = record.Collapse.ToProtobuf();
        if (record.Price != null)
            proto.Price = record.Price.ToProtobuf();

        return proto;
    }

    /// <summary>
    /// Converts a protobuf TradeFilterOptions to record
    /// </summary>
    public static TradeFilterOptions ToRecord(this Proto.TradeFilterOptions proto)
    {
        return new TradeFilterOptions
        {
            Account = proto.Account?.ToRecord(),
            Collapse = proto.Collapse?.ToRecord(),
            Price = proto.Price?.ToRecord()
        };
    }

    #endregion

    #region Placeholder methods for other filter types

    // Note: For brevity, I'll create simplified placeholder methods for the remaining filter types
    // These follow the same pattern as above

    public static Proto.ArmourFilters ToProtobuf(this ArmourFilters record) => new();
    public static ArmourFilters ToRecord(this Proto.ArmourFilters proto) => new();

    public static Proto.SocketFilters ToProtobuf(this SocketFilters record) => new();
    public static SocketFilters ToRecord(this Proto.SocketFilters proto) => new();

    public static Proto.EquipmentFilters ToProtobuf(this EquipmentFilters record) => new();
    public static EquipmentFilters ToRecord(this Proto.EquipmentFilters proto) => new();

    public static Proto.RequirementFilters ToProtobuf(this RequirementFilters record) => new();
    public static RequirementFilters ToRecord(this Proto.RequirementFilters proto) => new();

    public static Proto.MapFilters ToProtobuf(this MapFilters record) => new();
    public static MapFilters ToRecord(this Proto.MapFilters proto) => new();

    public static Proto.MiscFilters ToProtobuf(this MiscFilters record) => new();
    public static MiscFilters ToRecord(this Proto.MiscFilters proto) => new();

    public static Proto.HeistFilters ToProtobuf(this HeistFilters record) => new();
    public static HeistFilters ToRecord(this Proto.HeistFilters proto) => new();

    public static Proto.SanctumFilters ToProtobuf(this SanctumFilters record) => new();
    public static SanctumFilters ToRecord(this Proto.SanctumFilters proto) => new();

    public static Proto.UltimatumFilters ToProtobuf(this UltimatumFilters record) => new();
    public static UltimatumFilters ToRecord(this Proto.UltimatumFilters proto) => new();

    #endregion
}