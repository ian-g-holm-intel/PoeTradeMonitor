using System;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using PoeHudWrapper;
using PoeLib;
using PoeLib.Tools;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace TradeBotLib;

public interface IPriceValidator
{
    bool IsCorrectCurrencyPrice(CurrencyTradeOffer tradeOffer);
    bool IsCorrectItem(ItemTradeRequest tradeRequest, string itemName, Entity item, out int stackSize);
}

public class PriceValidator : IPriceValidator
{
    private readonly ICurrencyPriceCache currencyPriceCache;
    private readonly IPoeHudWrapper poeHud;
    private readonly ILogger<PriceValidator> log;

    public PriceValidator(ICurrencyPriceCache currencyPriceCache, IPoeHudWrapper poeHud, ILogger<PriceValidator> log)
    {
        this.currencyPriceCache = currencyPriceCache;
        this.poeHud = poeHud;
        this.log = log;
    }

    public bool IsCorrectCurrencyPrice(CurrencyTradeOffer tradeOffer)
    {
        if (tradeOffer.Theirs.Type != CurrencyType.chaos && tradeOffer.Mine.Type != CurrencyType.chaos)
            return false;

        try
        {
            var myPrice = currencyPriceCache.GetPrice(tradeOffer.Mine.Type);
            var theirPrice = currencyPriceCache.GetPrice(tradeOffer.Theirs.Type);
            var currencyPrice = tradeOffer.Theirs.Type == CurrencyType.chaos ? myPrice.SellFraction : theirPrice.BuyFraction;
            var offerPrice = new Fraction(decimal.ToInt32(tradeOffer.Theirs.Amount), decimal.ToInt32(tradeOffer.Mine.Amount));
            return offerPrice.DecimalValue >= currencyPrice.DecimalValue;
        }
        catch (CurrencyPriceNotFoundException)
        {
            return false;
        }
    }

    public bool IsCorrectItem(ItemTradeRequest tradeRequest, string itemName, Entity item, out int stackSize)
    {
        if (item.HasComponent<Stack>() && item.GetComponent<Stack>().Size != 0)
            stackSize = item.GetComponent<Stack>().Size;
        else
            stackSize = 1;

        var tooltipLines = poeHud.GetTooltipLines();
        var expectedItemName = tradeRequest.Item.Name;
        var actualName = itemName;
        var actualBaseType = poeHud.GetBaseType(item);
        if(actualBaseType == "Imprinted Bestiary Orb")
        {
            return true;
        }

        if (!expectedItemName.Equals(actualName.Replace("’", "'"), StringComparison.OrdinalIgnoreCase))
        {
            log.LogInformation($"Name does not match: Expected - {expectedItemName}, Actual - {actualName}");
            return false;
        }

        var expectedNumLinks = tradeRequest.Item.NumLinks;
        var actualNumLinks = item.GetNumberLinks();
        if (actualNumLinks != expectedNumLinks)
        {
            log.LogInformation($"Number of links doesn't match: Expected - {expectedNumLinks}, Actual - {actualNumLinks}");
            throw new AttemptedScamException(tradeRequest.Item.Name);
        }

        var expectedNumSockets = tradeRequest.Item.NumSockets;
        var actualNumSockets = item.GetNumberSockets();
        if (expectedNumSockets != actualNumSockets)
        {
            log.LogInformation($"Number of sockets doesn't match: Expected - {expectedNumSockets}, Actual - {actualNumSockets}");
            return false;
        }

        var expectedCorruption = tradeRequest.Item.corrupted;
        var actualCorruption = item.IsCorrupted();
        if (expectedCorruption != actualCorruption)
        {
            log.LogInformation($"Corruption doesn't match: Expected - {expectedCorruption}, Actual - {actualCorruption}");
            return false;
        }

        var className = poeHud.GetClassName(item);
        if (className != "StackableCurrency" && className != "DivinationCard" && className != "Support Skill Gem")
        {
            var expectedBaseType = tradeRequest.Item.BaseType;
            if (!expectedBaseType.Contains(actualBaseType))
            {
                log.LogInformation($"BaseType does not match: Expected - {expectedBaseType}, Actual - {actualBaseType}");
                return false;
            }

            var expectedExplicitMods = tradeRequest.Item.rawExplicitMods.SelectMany(mod => mod.Split('\n', '\r', StringSplitOptions.RemoveEmptyEntries)).ToList();
            foreach (var explicitMod in expectedExplicitMods)
            {
                if (!tooltipLines.Any(line => explicitMod.StartsWith(line)))
                {
                    log.LogInformation($"Explicit mods don't match: Expected:" + Environment.NewLine + string.Join(Environment.NewLine, expectedExplicitMods) + Environment.NewLine + "Actual: " + Environment.NewLine + string.Join(Environment.NewLine, tooltipLines));
                    return false;
                }
            }
        }
        return true;
    }
}
