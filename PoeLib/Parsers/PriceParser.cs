using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace PoeLib.Parsers;

public interface IPriceParser
{
    Price Parse(string note, int chaosToDivineRatio);
}

public class PriceParser : IPriceParser
{
    private readonly Regex prefixPattern = new Regex(@"^(~b\/o)|(~price)", RegexOptions.Compiled);
    private readonly Regex currencyPattern = new Regex(@"\d[.,\d]*\s[\w]+", RegexOptions.Compiled);
    private readonly Regex amountPattern = new Regex(@"\d[.,\d]*", RegexOptions.Compiled);
    private readonly Regex typePattern = new Regex(@"(?<=\d|\.|,)[\s][a-zA-Z]+", RegexOptions.Compiled);
    private readonly ILogger<PriceParser> logger;

    public PriceParser(ILogger<PriceParser> logger)
    {
        this.logger = logger;
    }

    public Price Parse(string note, int chaosToDivineRatio)
    {
        var price = new Price();
        try
        {
            var inputString = note;
            if (string.IsNullOrEmpty(inputString) || !prefixPattern.IsMatch(inputString)) 
                return price;

            inputString = prefixPattern.Replace(inputString, "").Trim();
            var currencyMatch = currencyPattern.Match(inputString);
            while (currencyMatch.Success)
            {
                var currencyString = currencyMatch.ToString().Trim();
                var amountString = amountPattern.Match(currencyString).ToString().Trim();
                var currency = new Currency { Amount = decimal.Parse(amountString) };
                var typeString = typePattern.Match(currencyString).ToString().Trim().ToLower();
                var currencyTypes = (CurrencyType[]) Enum.GetValues(typeof(CurrencyType));
                var currencyType = currencyTypes.SingleOrDefault(type => type.GetCurrencyTradeName().Equals(typeString, StringComparison.InvariantCultureIgnoreCase) || type.ToString().Equals(typeString, StringComparison.InvariantCultureIgnoreCase));
                if (currencyType != CurrencyType.none)
                {
                    currency.Type = currencyType;
                    price.Currencies.Add(currency);
                }
                currencyMatch = currencyMatch.NextMatch();
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to parse price: {note}: {ex}");
        }
        return price;
    }
}

[DataContract]
public class Price : IEquatable<Price>
{
    [DataMember]
    public List<Currency> Currencies { get; set; } = new List<Currency>();

    public Price() { }

    public Price(decimal priceInChaos)
    {
        Currencies.Add(new Currency { Type = CurrencyType.chaos, Amount = Math.Round(priceInChaos) });
    }
	
	public Price(CurrencyType type, decimal amount)
    {
        Currencies.Add(new Currency { Type = type, Amount = amount });
    }

    public Price(decimal priceInChaos, int chaosToDivineRatio)
    {
        if (priceInChaos >= chaosToDivineRatio)
        {
            var priceInDivines = priceInChaos / chaosToDivineRatio;
            Currencies.Add(new Currency { Type = CurrencyType.divine, Amount = Math.Round(priceInDivines, 1) });
        }
        else
        {
            Currencies.Add(new Currency { Type = CurrencyType.chaos, Amount = Math.Round(priceInChaos) });
        }
    }

    public decimal PriceInDivine(decimal divineRate)
    {
        decimal divines = (from Currency currency in Currencies where currency.Type == CurrencyType.divine select currency.Amount).FirstOrDefault();
        decimal chaos = (from Currency currency in Currencies where currency.Type == CurrencyType.chaos select currency.Amount).FirstOrDefault();
        return divineRate == 0 ? 0 : divines + chaos / divineRate;
    }

    public decimal PriceInChaos(decimal divineRate)
    {
        decimal divines = (from Currency currency in Currencies where currency.Type == CurrencyType.divine select currency.Amount).FirstOrDefault();
        decimal chaos = (from Currency currency in Currencies where currency.Type == CurrencyType.chaos select currency.Amount).FirstOrDefault();
        return chaos + divines * divineRate;
    }

    public override string ToString()
    {
        if (Currencies.Count == 0) return "";

        string output = "";
        foreach (var currency in Currencies.Where(c => c.Amount != 0))
            output += $" {currency.Amount} {currency.Type}";
        return output.TrimStart();
    }

    public bool Equals(Price other)
    {
        if (other is null) return false;
        if (Currencies.Count != other.Currencies.Count) return false;

        return Currencies.All(currency =>
            other.Currencies.Any(otherCurrency =>
                otherCurrency.Type == currency.Type &&
                otherCurrency.Amount == currency.Amount));
    }

    public override bool Equals(object obj)
    {
        if (obj is Price price)
            return Equals(price);
        return false;
    }

    public override int GetHashCode()
    {
        return Currencies.Aggregate(0, (hash, currency) =>
            hash ^ (currency.Type.GetHashCode() * 17 + currency.Amount.GetHashCode()));
    }
}
