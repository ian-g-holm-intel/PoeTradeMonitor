namespace PoeTrade.Contracts.Extensions;

/// <summary>
/// Extension methods to convert between C# request records and protobuf classes
/// </summary>
public static class ProtobufRequestConversionExtensions
{
    #region TradeSearchRequest Conversions

    /// <summary>
    /// Converts a TradeSearchRequest record to protobuf message
    /// </summary>
    public static Proto.TradeSearchRequest ToProtobuf(this TradeSearchRequest record)
    {
        return new Proto.TradeSearchRequest
        {
            Query = record.Query.ToProtobuf(),
            Sort = record.Sort.ToProtobuf()
        };
    }

    /// <summary>
    /// Converts a protobuf TradeSearchRequest to record
    /// </summary>
    public static TradeSearchRequest ToRecord(this Proto.TradeSearchRequest proto)
    {
        return new TradeSearchRequest
        {
            Query = proto.Query.ToRecord(),
            Sort = proto.Sort.ToRecord()
        };
    }

    #endregion

    #region TradeQuery Conversions

    /// <summary>
    /// Converts a TradeQuery record to protobuf message
    /// </summary>
    public static Proto.TradeQuery ToProtobuf(this TradeQuery record)
    {
        var proto = new Proto.TradeQuery
        {
            Status = record.Status.ToProtobuf()
        };

        proto.Stats.AddRange(record.Stats.Select(s => s.ToProtobuf()));
        proto.Filters = record.Filters.ToProtobuf();

        return proto;
    }

    /// <summary>
    /// Converts a protobuf TradeQuery to record
    /// </summary>
    public static TradeQuery ToRecord(this Proto.TradeQuery proto)
    {
        return new TradeQuery
        {
            Status = proto.Status.ToRecord(),
            Stats = proto.Stats.Select(s => s.ToRecord()).ToList(),
            Filters = proto.Filters.ToRecord()
        };
    }

    #endregion

    #region StatusFilter Conversions

    /// <summary>
    /// Converts a StatusFilter record to protobuf message
    /// </summary>
    public static Proto.StatusFilter ToProtobuf(this StatusFilter record)
    {
        return new Proto.StatusFilter
        {
            Option = record.Option
        };
    }

    /// <summary>
    /// Converts a protobuf StatusFilter to record
    /// </summary>
    public static StatusFilter ToRecord(this Proto.StatusFilter proto)
    {
        return new StatusFilter
        {
            Option = proto.Option
        };
    }

    #endregion

    #region StatsFilter Conversions

    /// <summary>
    /// Converts a StatsFilter record to protobuf message
    /// </summary>
    public static Proto.StatsFilter ToProtobuf(this StatsFilter record)
    {
        var proto = new Proto.StatsFilter
        {
            Type = record.Type
        };

        // Convert object filters to strings (simplified approach)
        proto.Filters.AddRange(record.Filters.Select(f => f?.ToString() ?? string.Empty));

        return proto;
    }

    /// <summary>
    /// Converts a protobuf StatsFilter to record
    /// </summary>
    public static StatsFilter ToRecord(this Proto.StatsFilter proto)
    {
        return new StatsFilter
        {
            Type = proto.Type,
            Filters = proto.Filters.Cast<object>().ToList()
        };
    }

    #endregion

    #region SortOptions Conversions

    /// <summary>
    /// Converts a SortOptions record to protobuf message
    /// </summary>
    public static Proto.SortOptions ToProtobuf(this SortOptions record)
    {
        var proto = new Proto.SortOptions();

        if (!string.IsNullOrEmpty(record.Price))
            proto.Price = record.Price;

        return proto;
    }

    /// <summary>
    /// Converts a protobuf SortOptions to record
    /// </summary>
    public static SortOptions ToRecord(this Proto.SortOptions proto)
    {
        return new SortOptions
        {
            Price = proto.Price
        };
    }

    #endregion

    #region QueryFilters Conversions

    /// <summary>
    /// Converts a QueryFilters record to protobuf message
    /// </summary>
    public static Proto.QueryFilters ToProtobuf(this QueryFilters record)
    {
        var proto = new Proto.QueryFilters();

        if (record.TypeFilters != null)
            proto.TypeFilters = record.TypeFilters.ToProtobuf();
        if (record.WeaponFilters != null)
            proto.WeaponFilters = record.WeaponFilters.ToProtobuf();
        if (record.ArmourFilters != null)
            proto.ArmourFilters = record.ArmourFilters.ToProtobuf();
        if (record.SocketFilters != null)
            proto.SocketFilters = record.SocketFilters.ToProtobuf();
        if (record.HeistFilters != null)
            proto.HeistFilters = record.HeistFilters.ToProtobuf();
        if (record.SanctumFilters != null)
            proto.SanctumFilters = record.SanctumFilters.ToProtobuf();
        if (record.UltimatumFilters != null)
            proto.UltimatumFilters = record.UltimatumFilters.ToProtobuf();
        if (record.EquipmentFilters != null)
            proto.EquipmentFilters = record.EquipmentFilters.ToProtobuf();
        if (record.ReqFilters != null)
            proto.ReqFilters = record.ReqFilters.ToProtobuf();
        if (record.MapFilters != null)
            proto.MapFilters = record.MapFilters.ToProtobuf();
        if (record.MiscFilters != null)
            proto.MiscFilters = record.MiscFilters.ToProtobuf();
        if (record.TradeFilters != null)
            proto.TradeFilters = record.TradeFilters.ToProtobuf();

        return proto;
    }

    /// <summary>
    /// Converts a protobuf QueryFilters to record
    /// </summary>
    public static QueryFilters ToRecord(this Proto.QueryFilters proto)
    {
        return new QueryFilters
        {
            TypeFilters = proto.TypeFilters?.ToRecord(),
            WeaponFilters = proto.WeaponFilters?.ToRecord(),
            ArmourFilters = proto.ArmourFilters?.ToRecord(),
            SocketFilters = proto.SocketFilters?.ToRecord(),
            HeistFilters = proto.HeistFilters?.ToRecord(),
            SanctumFilters = proto.SanctumFilters?.ToRecord(),
            UltimatumFilters = proto.UltimatumFilters?.ToRecord(),
            EquipmentFilters = proto.EquipmentFilters?.ToRecord(),
            ReqFilters = proto.ReqFilters?.ToRecord(),
            MapFilters = proto.MapFilters?.ToRecord(),
            MiscFilters = proto.MiscFilters?.ToRecord(),
            TradeFilters = proto.TradeFilters?.ToRecord()
        };
    }

    #endregion

    #region Common Filter Type Conversions

    /// <summary>
    /// Converts a RangeFilter record to protobuf message
    /// </summary>
    public static Proto.RangeFilter ToProtobuf(this RangeFilter record)
    {
        var proto = new Proto.RangeFilter();

        if (record.Min.HasValue)
            proto.Min = record.Min.Value;
        if (record.Max.HasValue)
            proto.Max = record.Max.Value;

        return proto;
    }

    /// <summary>
    /// Converts a protobuf RangeFilter to record
    /// </summary>
    public static RangeFilter ToRecord(this Proto.RangeFilter proto)
    {
        return new RangeFilter
        {
            Min = proto.Min,
            Max = proto.Max
        };
    }

    /// <summary>
    /// Converts an OptionFilter record to protobuf message
    /// </summary>
    public static Proto.OptionFilter ToProtobuf(this OptionFilter record)
    {
        return new Proto.OptionFilter
        {
            Option = record.Option ?? string.Empty
        };
    }

    /// <summary>
    /// Converts a protobuf OptionFilter to record
    /// </summary>
    public static OptionFilter ToRecord(this Proto.OptionFilter proto)
    {
        return new OptionFilter
        {
            Option = string.IsNullOrEmpty(proto.Option) ? null : proto.Option
        };
    }

    /// <summary>
    /// Converts an InputFilter record to protobuf message
    /// </summary>
    public static Proto.InputFilter ToProtobuf(this InputFilter record)
    {
        return new Proto.InputFilter
        {
            Input = record.Input ?? string.Empty
        };
    }

    /// <summary>
    /// Converts a protobuf InputFilter to record
    /// </summary>
    public static InputFilter ToRecord(this Proto.InputFilter proto)
    {
        return new InputFilter
        {
            Input = string.IsNullOrEmpty(proto.Input) ? null : proto.Input
        };
    }

    /// <summary>
    /// Converts a PriceFilter record to protobuf message
    /// </summary>
    public static Proto.PriceFilter ToProtobuf(this PriceFilter record)
    {
        var proto = new Proto.PriceFilter
        {
            Option = record.Option ?? string.Empty
        };

        if (record.Min.HasValue)
            proto.Min = record.Min.Value;
        if (record.Max.HasValue)
            proto.Max = record.Max.Value;

        return proto;
    }

    /// <summary>
    /// Converts a protobuf PriceFilter to record
    /// </summary>
    public static PriceFilter ToRecord(this Proto.PriceFilter proto)
    {
        return new PriceFilter
        {
            Option = string.IsNullOrEmpty(proto.Option) ? null : proto.Option,
            Min = proto.Min,
            Max = proto.Max
        };
    }

    #endregion
}