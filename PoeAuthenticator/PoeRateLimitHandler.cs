using ComposableAsync;

namespace PoeAuthenticator;

public class PoeRateLimitHandler : DelegatingHandler
{
    private readonly IPoeRateLimitService _rateLimitService;

    public PoeRateLimitHandler(IPoeRateLimitService rateLimitService)
    {
        _rateLimitService = rateLimitService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var endpoint = GetEndpointType(request.RequestUri!);
        var limiter = _rateLimitService.GetLimiter(endpoint);
        if (limiter != null)
        {
            await limiter;
        }

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        await _rateLimitService.CheckRateLimitAsync(response).ConfigureAwait(false);
        return response;
    }

    private string GetEndpointType(Uri uri)
    {
        var path = uri?.AbsolutePath.ToLower() ?? "";
        return path switch
        {
            var p when p.Contains("/trade2/search") => "search",
            var p when p.Contains("/trade2/fetch") => "fetch",
            var p when p.Contains("/trade2/exchange") => "exchange",
            var p when p.Contains("/trade2/whisper") => "whisper",
            var p when p.Contains("/leagues") => "leagues",
            _ => "backend"
        };
    }
}