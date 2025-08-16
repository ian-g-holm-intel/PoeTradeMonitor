using Microsoft.Extensions.Logging;
using System.Net;

namespace PoeAuthenticator;

/// <summary>
/// Extension methods for CookieContainer to handle Path of Exile authentication cookies.
/// </summary>
public static class CookieExtensionMethods
{
    /// <summary>
    /// Updates the cookie container with new Path of Exile authentication cookies.
    /// </summary>
    /// <param name="cookieContainer">The cookie container to update.</param>
    /// <param name="poeCookies">Dictionary of cookie names and values to add or update.</param>
    /// <param name="logger">Optional logger for tracking cookie updates.</param>
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
