using Microsoft.Extensions.DependencyInjection;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper;

public class Bootstrapper
{
    public void RegisterServices(IServiceCollection container)
    {
        container.AddSingleton<IMemoryProvider, MemoryProvider>();
        container.AddSingleton<IGameWrapper, GameWrapper>();
        container.AddSingleton<IPoeHudWrapper, PoeHudWrapper>();
    }
}
