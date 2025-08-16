using System.Linq;
using System.Threading.Tasks;
using LiquidState;
using LiquidState.Awaitable.Core;
using PoeHudWrapper;
using System;

namespace PoeCrafter;

public interface IRarityStateMachine
{
    Task GotoState(State state);
    Task ChangeState(Trigger trigger);
}

public class RarityStateMachine : IRarityStateMachine
{
    private readonly IAwaitableStateMachine<State, Trigger> machine;
    protected readonly ITradeCommands tradeCommands;
    protected readonly IPoeHudWrapper poeHud;

    protected virtual async Task UseCurrency(TradeCurrencyType type)
    {
        var currencyLocation = poeHud.StashCurrencies.First(currency => currency.Type == type).Location;
        var itemLocation = poeHud.CraftingSlotLocation;
        await tradeCommands.RightClickMouse(currencyLocation);
        await tradeCommands.LeftClickMouse(itemLocation);
        await Task.Delay(30);
    }

    protected virtual Task UseEssence()
    {
        return Task.CompletedTask;
    }

    public RarityStateMachine(ITradeCommands tradeCommands, IPoeHudWrapper poeHud)
    {
        this.tradeCommands = tradeCommands;
        this.poeHud = poeHud;
        var config = StateMachineFactory.CreateAwaitableConfiguration<State, Trigger>();

        config.ForState(State.Normal)
            .OnEntry(() => Console.WriteLine("Normal"))
            .Permit(Trigger.Transmute, State.Magic, async () => await UseCurrency(TradeCurrencyType.Transmute))
            .Permit(Trigger.Alch, State.Rare, async () => await UseCurrency(TradeCurrencyType.Alch))
            .Permit(Trigger.Essence, State.Rare, UseEssence);

        config.ForState(State.Magic)
            .OnEntry(() => Console.WriteLine("Magic"))
            .Permit(Trigger.Regal, State.Rare, async () => await UseCurrency(TradeCurrencyType.Regal));

        machine = StateMachineFactory.Create(State.Normal, config);
    }

    public async Task GotoState(State state)
    {
        await machine.MoveToStateAsync(state);
    }

    public async Task ChangeState(Trigger trigger)
    {
        await machine.FireAsync(trigger);
    }
}

public enum State
{
    Normal,
    Magic,
    Rare
}

public enum Trigger
{
    Transmute,
    Regal,
    Alch,
    Essence,
    Scour
}
