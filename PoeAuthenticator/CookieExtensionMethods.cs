using Microsoft.Extensions.Logging;
using System.Net;

namespace PoeAuthenticator;

public static class CookieExtensionMethods
{
    public static void UpdateCookies(this CookieContainer cookieContainer, Dictionary<string, string> poeCookies, ILogger? logger = default)
    {
        var existingCookies = cookieContainer.GetCookies(new Uri("https://www.pathofexile.com")).Cast<Cookie>();
        foreach (var poeCookie in poeCookies)
        {
            var existingCookie = existingCookies.SingleOrDefault(c => c.Name == poeCookie.Key);
            if (existingCookie == null || existingCookie.Value != poeCookie.Value)
            {
                if (logger != null)
                    logger.LogInformation($"Updating Cookie: {poeCookie.Key} = {poeCookie.Value}");
                var cookie = new Cookie(poeCookie.Key, poeCookie.Value, "/", ".pathofexile.com") { HttpOnly = true, Secure = true };
                cookieContainer.Add(cookie);
            }
        }
    }
}
