using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using PoeHudWrapper;
using PoeLib.Common;

namespace PoeTradeMonitor.Service;

public interface IPriceValidator
{
    bool IsCorrectItem(ItemTradeRequest tradeRequest, string itemName, Entity item, out int stackSize);
}

public class PriceValidator : IPriceValidator
{
    private readonly IPoeHudWrapper poeHud;
    private readonly ILogger<PriceValidator> log;

    public PriceValidator(IPoeHudWrapper poeHud, ILogger<PriceValidator> log)
    {
        this.poeHud = poeHud;
        this.log = log;
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

        var expectedNumLinks = tradeRequest.Item.MaxLinks;
        var actualNumLinks = item.GetNumberLinks();
        if (actualNumLinks != expectedNumLinks)
        {
            log.LogInformation($"Number of links doesn't match: Expected - {expectedNumLinks}, Actual - {actualNumLinks}");
            throw new AttemptedScamException(tradeRequest.Item.Name);
        }

        var expectedNumSockets = tradeRequest.Item.Sockets?.Count ?? 0;
        var actualNumSockets = item.GetNumberSockets();
        if (expectedNumSockets != actualNumSockets)
        {
            log.LogInformation($"Number of sockets doesn't match: Expected - {expectedNumSockets}, Actual - {actualNumSockets}");
            return false;
        }

        var expectedCorruption = tradeRequest.Item.Corrupted ?? false;
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

            var expectedExplicitMods = tradeRequest.Item.ExplicitMods?.SelectMany(mod => mod.RawModText.Split('\n', '\r', StringSplitOptions.RemoveEmptyEntries)).ToArray() ?? Array.Empty<string>();
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
