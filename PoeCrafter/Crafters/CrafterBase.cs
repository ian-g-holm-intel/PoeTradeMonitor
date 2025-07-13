using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using PoeLib;
using ExileCore.Shared.Enums;
using TradeBotLib;
using PoeHudWrapper;
using InputSimulatorStandard.Native;
using PoeHudWrapper.MemoryObjects;
using System.Threading;
using MoreLinq;

namespace PoeCrafter.Crafters;

public abstract class CrafterBase : ICrafter
{
    protected readonly IPoeHudWrapper poeHud;
    private readonly ITradeCommands tradeCommands;
    private readonly IRarityStateMachine stateMachine;
    private CurrencyType currencySpam = CurrencyType.none;
    private string mostRecentMods = "";
    
    public CrafterBase(IPoeHudWrapper phw, ITradeCommands tc, IRarityStateMachine rsm)
    {
        poeHud = phw;
        tradeCommands = tc;
        stateMachine = rsm;
    }

    public abstract Task Craft();
    protected abstract int GetNumberOfRemainingPrefixes();

    protected abstract int GetNumberOfRemainingSuffixes();

    protected virtual Task Setup()
    {
        return Task.CompletedTask;
    }

    protected virtual bool HasRemainingMods()
    {
        return GetNumberOfPrefixes() < 3 && GetNumberOfRemainingPrefixes() > 0 || GetNumberOfSuffixes() < 3 && GetNumberOfRemainingSuffixes() > 0;
    }

    protected int GetNumberOfPrefixes()
    {
        return GetCraftingMods().Count(mod => mod.AffixType == ExileCore.Shared.Enums.ModType.Prefix);
    }

    protected int GetNumberOfSuffixes()
    {
        return GetCraftingMods().Count(mod => mod.AffixType == ExileCore.Shared.Enums.ModType.Suffix);
    }

    protected int GetNumberOfAffixes()
    {
        return GetNumberOfPrefixes() + GetNumberOfSuffixes();
    }

    protected async Task UseCurrency(CurrencyType type)
    {
        if (!HasCurrency(type))
            throw new CurrencyNotFoundException();

        await CurrencySpamCheck(async () =>
        {
            var currencyLocation = GetCurrencyLocation(type);
            await tradeCommands.RightClickMouse(currencyLocation);
            await ClickItem();
        });
    }

    protected async Task UseEssence(CurrencyType type)
    {
        if (!HasEssence(type))
            throw new CurrencyNotFoundException();

        await EssenceSpamCheck(async () =>
        {
            var currencyLocation = GetEssenceLocation(type);
            await tradeCommands.RightClickMouse(currencyLocation);
            await ClickEssenceItem();
        });
    }

    protected async Task StartUsingCurrency(CurrencyType type)
    {
        currencySpam = type;
        var itemLocation = GetItemLocation();
        var currencyLocation = GetEssenceLocation(type);
        await tradeCommands.RightClickMouse(currencyLocation);
        tradeCommands.MouseMove(itemLocation);
        await tradeCommands.SendKeyDown(VirtualKeyCode.SHIFT);
    }

    protected async Task StartUsingEssence(CurrencyType type)
    {
        currencySpam = type;
        var itemLocation = GetEssenceItemLocation();
        var currencyLocation = GetEssenceLocation(type);
        await tradeCommands.RightClickMouse(currencyLocation);
        tradeCommands.MouseMove(itemLocation);
        await tradeCommands.SendKeyDown(VirtualKeyCode.SHIFT);
    }

    protected async Task StopUsingCurrency()
    {
        currencySpam = CurrencyType.none;
        var itemLocation = GetItemLocation();
        await tradeCommands.SendKeyUp(VirtualKeyCode.SHIFT);
        await tradeCommands.RightClickMouse(itemLocation);
    }

    protected async Task StopUsingEssence()
    {
        currencySpam = CurrencyType.none;
        var itemLocation = GetEssenceItemLocation();
        await tradeCommands.SendKeyUp(VirtualKeyCode.SHIFT);
        await tradeCommands.RightClickMouse(itemLocation);
    }

    protected async Task ClickItem()
    {
        var itemLocation = GetItemLocation();
        await tradeCommands.LeftClickMouse(itemLocation);
        await WaitModsChanged();
    }

    protected async Task ClickEssenceItem()
    {
        var itemLocation = GetEssenceItemLocation();
        await tradeCommands.LeftClickMouse(itemLocation);
        await WaitEssenceModsChanged();
    }

    private async Task CurrencySpamCheck(Func<Task> action)
    {
        var tempCurrencySpam = currencySpam;
        await StopUsingCurrency();
        await action();
        if (tempCurrencySpam != CurrencyType.none)
            await StartUsingCurrency(tempCurrencySpam);
    }

    private async Task EssenceSpamCheck(Func<Task> action)
    {
        var tempCurrencySpam = currencySpam;
        await StopUsingEssence();
        await action();
        if (tempCurrencySpam != CurrencyType.none)
            await StartUsingEssence(tempCurrencySpam);
    }

    private Point GetCurrencyLocation(CurrencyType type)
    {
        var currencies = GetCurrencies().Where(currency => currency.Type == type);
        var currencyStacks = currencies.ToList();
        if (!currencyStacks.Any())
            throw new NotEnoughCurrencyException(type);
        return currencyStacks.First().Location;
    }

    private Point GetEssenceLocation(CurrencyType type)
    {
        var currencies = GetEssences().Where(currency => currency.Type == type);
        var currencyStacks = currencies.ToList();
        if (!currencyStacks.Any())
            throw new NotEnoughCurrencyException(type);
        return currencyStacks.First().Location;
    }

    private string GetModsString()
    {
        var mods = GetCraftingMods();
        while (mods.Any(m => m.Record.StatNames.Length > m.StatValue.Length))
        {
            Thread.Sleep(100);
            mods = GetCraftingMods();
        }

        var modString = "";
        foreach (var mod in mods.Where(x => x.AffixType == ExileCore.Shared.Enums.ModType.Prefix || x.AffixType == ExileCore.Shared.Enums.ModType.Suffix))
        {
            modString += mod.AffixText;
            for (int i = 0; i < mod.Record.StatNames.Length; i++)
            {
                if (mod.Record.StatNames[i] != null)
                    modString += mod.StatValue[i];
            }

        }
        return modString;
    }

    private string GetEssenceModsString()
    {
        var mods = GetEssenceCraftingMods();
        var modString = "";
        foreach (var mod in mods.Where(x => x.AffixType == ExileCore.Shared.Enums.ModType.Prefix || x.AffixType == ExileCore.Shared.Enums.ModType.Suffix))
        {
            modString += mod.AffixText;
            for (int i = 0; i < mod.Record.StatNames.Length; i++)
            {
                if (mod.Record.StatNames[i] != null)
                    modString += mod.StatValue[i];
            }

        }
        return modString;
    }

    protected async Task WaitModsChanged()
    {
        var mods = GetModsString();
        using var ctSource = new CancellationTokenSource(1000);
        while ((string.IsNullOrEmpty(mods) || mods == mostRecentMods) && !ctSource.IsCancellationRequested)
        {
            await Task.Delay(10);
            mods = GetModsString();
        }
        mostRecentMods = mods;
    }

    protected async Task WaitEssenceModsChanged()
    {
        var mods = GetEssenceModsString();
        using var ctSource = new CancellationTokenSource(1000);
        while ((string.IsNullOrEmpty(mods) || mods == mostRecentMods) && !ctSource.IsCancellationRequested)
        {
            await Task.Delay(10);
            mods = GetEssenceModsString();
        }
        mostRecentMods = mods;
    }

    protected bool HasCurrency(CurrencyType type)
    {
        try
        {
            for (int i = 0; i < 5; i++)
            {
                var currencyAmount = GetCurrencyCount(type);
                if (currencyAmount > 0)
                    return true;
                Thread.Sleep(100);
            }
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new CurrencyNotFoundException();
        }
    }

    protected decimal GetCurrencyCount(CurrencyType type)
    {
        try
        {
            return GetCurrencies().Where(c => c.Type == type).Sum(c => c.Amount);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new CurrencyNotFoundException();
        }
    }

    protected bool HasEssence(CurrencyType type)
    {
        try
        {
            for (int i = 0; i < 5; i++)
            {
                var currencyAmount = GetEssenceCount(type);
                if (currencyAmount > 0)
                    return true;
                Thread.Sleep(100);
            }
            return false;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new CurrencyNotFoundException();
        }
    }

    protected decimal GetEssenceCount(CurrencyType type)
    {
        try
        {
            return GetEssences().Where(c => c.Type == type).Sum(c => c.Amount);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new CurrencyNotFoundException();
        }
    }

    protected bool HasEssences => GetEssences().Any();

    protected bool HasPathToRare => HasCurrency(CurrencyType.alch) || HasEssences || HasCurrency(CurrencyType.transmute) && HasCurrency(CurrencyType.regal);

    protected virtual List<CurrencyStack> GetCurrencies()
    {
        try
        {
            return poeHud.StashCurrencies;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new CurrencyNotFoundException();
        }
    }

    protected virtual List<CurrencyStack> GetEssences()
    {
        try
        {
            return poeHud.StashEssences;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new CurrencyNotFoundException();
        }
    }

    protected virtual ModValueWrapper[] GetCraftingMods()
    {
        return poeHud.CraftingSlotMods;
    }

    protected virtual ModValueWrapper[] GetEssenceCraftingMods()
    {
        return poeHud.EssenceCraftingSlotMods;
    }

    protected virtual int GetModCount()
    {
        try
        {
            return GetCraftingMods().Length;
        }
        catch (NullReferenceException)
        {
            throw new ModsNotFoundException();
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ModsNotFoundException();
        }
    }

    protected virtual bool HasMods(Predicate<ModValueWrapper[]> predicate)
    {
        var modValues = GetCraftingMods();
        return predicate.Invoke(modValues);
    }

    protected virtual Point GetItemLocation()
    {
        return poeHud.CraftingSlotLocation;
    }

    protected virtual Point GetEssenceItemLocation()
    {
        return poeHud.EssenceCraftingSlotLocation;
    }

    protected async Task MakeRare()
    {
        if (poeHud.CraftingSlotRarity == ItemRarity.Normal)
        {   
            if (HasCurrency(CurrencyType.transmute) && HasCurrency(CurrencyType.regal))
            {
                await stateMachine.ChangeState(Trigger.Transmute);
                await stateMachine.ChangeState(Trigger.Regal);
            }
            else
            {
                throw new NotEnoughCurrencyToRareException();
            }
        }
        else if (poeHud.CraftingSlotRarity == ItemRarity.Magic)
        {
            if (HasCurrency(CurrencyType.regal))
            {
                await stateMachine.ChangeState(Trigger.Regal);
            }
            else
            {
                throw new NotEnoughCurrencyToRareException();
            }
        }
        else if (poeHud.CraftingSlotRarity == ItemRarity.Rare)
        {
            await stateMachine.GotoState(State.Rare);
        }
    }

    protected async Task MakeMagic()
    {
        if (poeHud.CraftingSlotRarity == ItemRarity.Normal)
        {
            if (HasCurrency(CurrencyType.transmute))
            {
                await stateMachine.ChangeState(Trigger.Transmute);
            }
            else
            {
                throw new NotEnoughCurrencyToRareException();
            }
        }
        else if (poeHud.CraftingSlotRarity == ItemRarity.Magic)
        {
            await stateMachine.GotoState(State.Magic);
        }
        else if (poeHud.CraftingSlotRarity == ItemRarity.Rare)
        {
            await MakeNormal();
            await MakeMagic();
        }
    }

    protected async Task MakeNormal()
    {
        if (HasCurrency(CurrencyType.scour))
            await stateMachine.ChangeState(Trigger.Scour);
        else
        {
            throw new NotEnoughCurrencyToRareException();
        }
    }
}
