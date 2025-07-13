using System.Linq;
using System.Threading.Tasks;
using log4net;
using LiquidState;
using LiquidState.Awaitable.Core;
using PoeLib;
using TradeBotLib;
using PoeHudWrapper;

namespace PoeCrafter;

public interface IRarityStateMachine
{
    Task GotoState(State state);
    Task ChangeState(Trigger trigger);
}

public class RarityStateMachine : IRarityStateMachine
{
    private readonly ILog log = LogManager.GetLogger(typeof(RarityStateMachine));
    private readonly IAwaitableStateMachine<State, Trigger> machine;
    protected readonly ITradeCommands tradeCommands;
    protected readonly IPoeHudWrapper poeHud;

    protected virtual async Task UseCurrency(CurrencyType type)
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
            .OnEntry(() => log.Info("Normal"))
            .PermitReentry(Trigger.Scour, async () => await UseCurrency(CurrencyType.scour))
            .Permit(Trigger.Transmute, State.Magic, async () => await UseCurrency(CurrencyType.transmute))
            .Permit(Trigger.Alch, State.Rare, async () => await UseCurrency(CurrencyType.alch))
            .Permit(Trigger.Essence, State.Rare, UseEssence);

        config.ForState(State.Magic)
            .OnEntry(() => log.Info("Magic"))
            .Permit(Trigger.Regal, State.Rare, async () => await UseCurrency(CurrencyType.regal));

        config.ForState(State.Rare)
            .OnEntry(() => log.Info("Rare"))
            .Permit(Trigger.Scour, State.Normal, async () => await UseCurrency(CurrencyType.scour));

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
