using System.ComponentModel;

namespace PoeTrade.Contracts.Extensions;

/// <summary>
/// Extension methods for enum types used in PoeTrade contracts.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the description attribute value for a TradeCurrencyType enum value.
    /// </summary>
    /// <param name="currency">The currency type to get description for.</param>
    /// <returns>The description from the DescriptionAttribute, or the enum name if no description exists.</returns>
    public static string GetCurrencyDescription(this TradeCurrencyType currency)
    {
        var fi = currency.GetType().GetField(currency.ToString());
        if (fi == null) return currency.ToString();
        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attributes.Length <= 0 ? currency.ToString() : attributes[0].Description;
    }
}
