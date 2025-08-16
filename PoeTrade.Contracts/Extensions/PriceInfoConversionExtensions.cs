namespace PoeTrade.Contracts.Extensions;

/// <summary>
/// Extension methods for converting between PriceInfo record and protobuf messages.
/// </summary>
public static class PriceInfoConversionExtensions
{
    /// <summary>
    /// Converts a PriceInfo record to protobuf message.
    /// </summary>
    /// <param name="record">The PriceInfo record to convert.</param>
    /// <returns>The protobuf PriceInfo message.</returns>
    public static Proto.PriceInfo ToProtobuf(this PriceInfo record)
    {
        var proto = new Proto.PriceInfo
        {
            PriceType = record.PriceType,
            Amount = (double)record.Amount,
            IsRelative = record.IsRelative
        };

        // Convert the enum currency type to string using the converter
        var currencyString = TradeCurrencyTypeConverter.GetStringValue(record.CurrencyType);
        if (!string.IsNullOrEmpty(currencyString))
        {
            proto.CurrencyType = currencyString;
        }

        return proto;
    }

    /// <summary>
    /// Converts a protobuf PriceInfo message to record.
    /// </summary>
    /// <param name="proto">The protobuf PriceInfo message to convert.</param>
    /// <returns>The PriceInfo record.</returns>
    public static PriceInfo ToRecord(this Proto.PriceInfo proto)
    {
        // Convert the string currency type to enum using the converter
        var currencyType = TradeCurrencyTypeConverter.GetEnumValue(proto.CurrencyType) ?? TradeCurrencyType.Unknown;

        return new PriceInfo
        {
            PriceType = proto.PriceType,
            Amount = (decimal)proto.Amount,
            CurrencyType = currencyType,
            IsRelative = proto.IsRelative
        };
    }
}