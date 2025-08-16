namespace PoeTrade.Contracts.Extensions;

/// <summary>
/// Extension methods for converting between CurrencyInfo record and protobuf messages.
/// </summary>
public static class CurrencyInfoConversionExtensions
{
    /// <summary>
    /// Converts a CurrencyInfo record to protobuf message.
    /// </summary>
    /// <param name="record">The CurrencyInfo record to convert.</param>
    /// <returns>The protobuf CurrencyInfo message.</returns>
    public static Proto.CurrencyInfo ToProtobuf(this CurrencyInfo record)
    {
        var proto = new Proto.CurrencyInfo
        {
            Amount = (double)record.Amount
        };

        // Convert the enum currency type to string using the converter
        var currencyString = TradeCurrencyTypeConverter.GetStringValue(record.Type);
        if (!string.IsNullOrEmpty(currencyString))
        {
            proto.Type = currencyString;
        }

        return proto;
    }

    /// <summary>
    /// Converts a protobuf CurrencyInfo message to record.
    /// </summary>
    /// <param name="proto">The protobuf CurrencyInfo message to convert.</param>
    /// <returns>The CurrencyInfo record.</returns>
    public static CurrencyInfo ToRecord(this Proto.CurrencyInfo proto)
    {
        // Convert the string currency type to enum using the converter
        var currencyType = TradeCurrencyTypeConverter.GetEnumValue(proto.Type) ?? TradeCurrencyType.Unknown;

        return new CurrencyInfo
        {
            Amount = (decimal)proto.Amount,
            Type = currencyType
        };
    }
}