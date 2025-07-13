using PoeLib;
using InputSimulatorStandard;
using Microsoft.Extensions.DependencyInjection;

namespace TradeBotLib;

public class Bootstrapper
{
    public void RegisterServices(IServiceCollection container)
    {
        container.AddSingleton<ITradeCommands, TradeCommands>();
        container.AddSingleton<IInputSimulator>(sp => new InputSimulator());
        container.AddSingleton<IPriceValidator, PriceValidator>();
        container.AddSingleton<ILocations, Locations2560x1440>();
    }
}
