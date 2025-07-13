using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.RegularExpressions;

namespace PoeAuthenticator;

public class LoginDelegateHandler : DelegatingHandler
{
    private const string LoginUri = "https://www.pathofexile.com/login/email?theme=league";
    private readonly ILogger<LoginDelegateHandler> logger;
    private readonly CookieContainer cookieContainer;

    public LoginDelegateHandler(ILogger<LoginDelegateHandler> logger, CookieContainer cookieContainer)
    {
        this.logger = logger;
        this.cookieContainer = cookieContainer;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        UpdateCookies(response);

        var contentString = string.Empty;
        if (response.Content != null && response.StatusCode == HttpStatusCode.OK)
        {
            contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized || contentString.Contains("Click here to sign in"))
        {
            logger.LogInformation("Logging in to pathofexile.com");

            // Perform login
            response = await LoginAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Found)
            {
                UpdateCookies(response);
                // Create a new request with the same properties as the original
                var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);
                CopyHeaders(request, newRequest);
                if (request.Content != null)
                {
                    newRequest.Content = await CloneContent(request.Content).ConfigureAwait(false);
                }
                return await base.SendAsync(newRequest, cancellationToken).ConfigureAwait(false);
            }
        }

        return response;
    }

    private void UpdateCookies(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var cookieHeader in cookies)
            {
                var cookieParts = cookieHeader.Split(';')[0].Split('=');
                if (cookieParts.Length == 2)
                {
                    var cookieName = cookieParts[0].Trim();
                    var cookieValue = cookieParts[1].Trim();
                    logger.LogInformation($"Updating Cookie: {cookieName} = {cookieValue}");
                    var cookie = new Cookie(cookieName, cookieValue, "/", ".pathofexile.com");
                    cookieContainer.Add(cookie);
                }
            }
        }
    }

    private async Task<string> GetCsrfToken(HttpRequestMessage originalRequest, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, LoginUri);
        CopyHeaders(originalRequest, request);

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var scriptPattern = @"csrf""\s*:\s*""([^""]+)""";
            var match = Regex.Match(html, scriptPattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }
        throw new Exception("Could not find CSRF token in login page");
    }

    private async Task<HttpResponseMessage> LoginAsync(HttpRequestMessage originalRequest, CancellationToken cancellationToken)
    {
        var email = Environment.GetEnvironmentVariable("POE_LOGIN_EMAIL")
            ?? throw new Exception("POE_LOGIN_EMAIL environment variable not set");
        var password = Environment.GetEnvironmentVariable("POE_LOGIN_PASSWORD")
            ?? throw new Exception("POE_LOGIN_PASSWORD environment variable not set");

        var csrf = await GetCsrfToken(originalRequest, cancellationToken).ConfigureAwait(false);

        var formData = new Dictionary<string, string>
        {
            { "login_email", email },
            { "login_password", password },
            { "remember_me", "1" },
            { "hash", csrf },
            { "theme", "league" }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, LoginUri);
        CopyHeaders(originalRequest, request);
        request.Headers.Add("Referer", "https://www.pathofexile.com/login");
        request.Content = new FormUrlEncodedContent(formData);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private static void CopyHeaders(HttpRequestMessage source, HttpRequestMessage destination)
    {
        foreach (var header in source.Headers)
        {
            destination.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
    }

    private static async Task<HttpContent> CloneContent(HttpContent content)
    {
        var ms = new MemoryStream();
        await content.CopyToAsync(ms).ConfigureAwait(false);
        ms.Position = 0;

        var clone = new StreamContent(ms);
        foreach (var header in content.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}