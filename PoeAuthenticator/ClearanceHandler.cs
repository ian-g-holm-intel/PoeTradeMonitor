using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using PoeAuthenticator.Services;
using System.Diagnostics;
using System.Net;

namespace PoeAuthenticator;

/// <summary>
/// HTTP delegating handler that manages Cloudflare clearance challenges for Path of Exile API requests.
/// Automatically launches a browser when clearance is needed and waits for cookie updates.
/// </summary>
public class ClearanceHandler : DelegatingHandler
{
    private readonly string bravePath;
    private readonly ILogger<ClearanceHandler> logger;
    private readonly ICookieMonitorService cookieMonitorService;
    private readonly AsyncManualResetEvent cookiesUpdated = new AsyncManualResetEvent(false);
    

    /// <summary>
    /// Initializes a new instance of the <see cref="ClearanceHandler"/> class.
    /// </summary>
    /// <param name="logger">Logger for the clearance handler.</param>
    /// <param name="cookieMonitorService">Service for monitoring cookie updates.</param>
    public ClearanceHandler(ILogger<ClearanceHandler> logger, ICookieMonitorService cookieMonitorService)
    {
        this.logger = logger;
        this.cookieMonitorService = cookieMonitorService;
        this.cookieMonitorService.CookiesUpdated += CookieMonitorService_CookiesUpdated;
        this.bravePath = Environment.GetEnvironmentVariable("BRAVE_BROWSER_PATH") ?? @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";
    }

    private void CookieMonitorService_CookiesUpdated()
    {
        cookiesUpdated.Set();
    }

    /// <summary>
    /// Sends an HTTP request and handles Cloudflare clearance challenges when encountered.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The HTTP response message.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            logger.LogWarning("Clearance challenge encountered");
            cookiesUpdated.Reset();
            var uri = $"{request.RequestUri!.AbsoluteUri.Replace(@"wss://www.pathofexile.com/api/trade2/live/poe2/", @"https://www.pathofexile.com/trade2/search/poe2/")}?action=cloudflareChallenge";
            await LaunchBraveAsync(uri, cancellationToken).ConfigureAwait(false);
            logger.LogWarning("Waiting for cookies to be updated");
            await cookiesUpdated.WaitAsync(cancellationToken).ConfigureAwait(false);
            logger.LogWarning("Cookies updated");
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        return response;
    }

    /// <summary>
    /// Launches the Brave browser with the specified URL to handle clearance challenges.
    /// </summary>
    /// <param name="url">The URL to open in the browser.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    private async Task LaunchBraveAsync(string url, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = bravePath,
            Arguments = $"{url} --profile-directory=\"Profile 1\"",
            UseShellExecute = true
        };

        var process = Process.Start(startInfo);

        if (process != null)
            await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
    }
}