using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using PoeAuthenticator.Services;
using System.Diagnostics;
using System.Net;

namespace PoeAuthenticator;

public class ClearanceHandler : DelegatingHandler
{
    private const string bravePath = @"C:\Program Files\BraveSoftware\Brave-Browser\Application\brave.exe";
    private readonly ILogger<ClearanceHandler> logger;
    private readonly ICookieMonitorService cookieMonitorService;
    private readonly AsyncManualResetEvent cookiesUpdated = new AsyncManualResetEvent(false);
    

    public ClearanceHandler(ILogger<ClearanceHandler> logger, ICookieMonitorService cookieMonitorService)
    {
        this.logger = logger;
        this.cookieMonitorService = cookieMonitorService;
        this.cookieMonitorService.CookiesUpdated += CookieMonitorService_CookiesUpdated;
    }

    private void CookieMonitorService_CookiesUpdated()
    {
        cookiesUpdated.Set();
    }

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