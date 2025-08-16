using ExileCore;
using Microsoft.Extensions.DependencyInjection;

namespace PoeHudWrapper;

public static class Bootstrapper
{
    public static IServiceCollection AddPoeHudWrapper(this IServiceCollection container)
    {
        container.AddSingleton<Core>();
        container.AddSingleton<IPoeHudWrapper, PoeHudWrapper>();
        return container;
    }
}
