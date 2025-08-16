using Microsoft.Extensions.DependencyInjection;
using PoeAuthenticator.Services;
using System.Net;

namespace PoeAuthenticator;

public static class Bootstrapper
{
    public static IServiceCollection AddPoeAuthenticator(this IServiceCollection services)
    {
        // Register CookieContainer as singleton
        services.AddSingleton<CookieContainer>();
        services.AddTransient<IPoeCookieReader, PoeCookieReader>();
        services.AddTransient<IAlphaVssService, AlphaVssService>();
        services.AddSingleton<IPoeRateLimitService, PoeRateLimitService>();
        services.AddSingleton<ICookieMonitorService, CookieMonitorService>();

        // Register the HttpClientHandler with ClearanceHandler
        services.AddTransient<HttpMessageHandler>(sp =>
        {
            var cookieContainer = sp.GetRequiredService<CookieContainer>();
            return new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.All
            };
        });

        // Register LoginHandler
        services.AddTransient<ClearanceHandler>();
        services.AddTransient<LoginDelegateHandler>();
        services.AddTransient<PoeRateLimitHandler>();

        return services;
    }
}