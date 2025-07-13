using System.Text.RegularExpressions;

namespace PoeLib.Parsers;

public interface ICurrencyInfoParser
{
    Currency Parse(string currencyInfo);
}

public class CurrencyInfoParser : ICurrencyInfoParser
{
    private readonly Regex currencyTypePattern = new Regex(@"(?<=Rarity: Currency\r\n)[\w ']+", RegexOptions.Compiled);
    private readonly Regex currencyAmountPattern = new Regex(@"(?<=Stack Size: )[\d,]+(?=/)", RegexOptions.Compiled);
    private readonly Regex hasPricePattern = new Regex(@"(?<=~price )\d+[/\d]*", RegexOptions.Compiled);
    private readonly Regex numeratorPattern = new Regex(@"\d+", RegexOptions.Compiled);
    private readonly Regex denominatorPattern = new Regex(@"(?<=/)\d+", RegexOptions.Compiled);
    public Currency Parse(string currencyInfo)
    {
        var currencyItem = new Currency();

        var currencyTypeMatch = currencyTypePattern.Match(currencyInfo);
        if (!currencyTypeMatch.Success)
            return null;
        
        currencyItem.Type = currencyTypeMatch.ToString().GetCurrencyType();

        var currencyAmountMatch = currencyAmountPattern.Match(currencyInfo);
        if (!currencyAmountMatch.Success)
            return null;

        currencyItem.Amount = int.Parse(currencyAmountMatch.ToString().Replace(",",""));

        var hasPriceMatch = hasPricePattern.Match(currencyInfo);
        currencyItem.HasPriceSet = hasPriceMatch.Success;
        if (hasPriceMatch.Success)
        {
            var numeratorString = numeratorPattern.Match(hasPriceMatch.ToString()).ToString();
            var denominatorString = denominatorPattern.Match(hasPriceMatch.ToString()).ToString();
            currencyItem.Price = new Fraction(decimal.ToInt32(decimal.Parse(numeratorString)), !string.IsNullOrEmpty(denominatorString) ? decimal.ToInt32(decimal.Parse(denominatorString)) : 1);
        }

        return currencyItem;
    }
}
