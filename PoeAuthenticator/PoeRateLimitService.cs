using Nito.AsyncEx;
using RateLimiter;
using System.Collections.Concurrent;
using System.Net;

namespace PoeAuthenticator;

public interface IPoeRateLimitService
{
    TimeLimiter WebSocketLimiter { get; }
    TimeLimiter? GetLimiter(string endpoint);
    Task CheckRateLimitAsync(HttpResponseMessage response);
}

public class PoeRateLimitService : IPoeRateLimitService
{
    private readonly ConcurrentDictionary<string, TimeLimiter> _limiters;

    public PoeRateLimitService()
    {
        _limiters = new ConcurrentDictionary<string, TimeLimiter>
        {
            ["backend"] = GetBackendTimeLimiter(),
            ["search"] = GetSearchTimeLimiter(),
            ["fetch"] = GetFetchTimeLimiter(),
            ["whisper"] = GetWhisperTimeLimiter(),
            ["leagues"] = GetLeaguesViewLimiter(),
            ["exchange"] = GetExchangeLimiter()
        };
    }
    public TimeLimiter WebSocketLimiter => _limiters["search"];

    public TimeLimiter? GetLimiter(string endpoint) => _limiters.TryGetValue(endpoint, out var limiter) ? limiter : null;

    public async Task CheckRateLimitAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            if (response.Headers.RetryAfter?.Delta != null)
            {
                var retryAfter = (TimeSpan)response.Headers.RetryAfter.Delta;
                await Task.Delay(retryAfter).ConfigureAwait(false);
            }
            else
            {
                bool foundLimit = false;
                if (response.Headers.TryGetValues("x-rate-limit-account-state", out var accountLimitState))
                {
                    foreach (var accountLimit in accountLimitState)
                    {
                        var limitParts = accountLimit.Split(":");
                        if (limitParts.Length == 3 && limitParts[2] != "0")
                        {
                            foundLimit = true;
                            var limitRemaining = int.Parse(limitParts[2]);
                            await Task.Delay(TimeSpan.FromSeconds(limitRemaining)).ConfigureAwait(false);
                        }
                    }
                }
                if (response.Headers.TryGetValues("x-rate-limit-ip-state", out var ipLimitState))
                {
                    foreach (var ipLimit in ipLimitState)
                    {
                        var limitParts = ipLimit.Split(":");
                        if (limitParts.Length == 3 && limitParts[2] != "0")
                        {
                            foundLimit = true;
                            var limitRemaining = int.Parse(limitParts[2]);
                            await Task.Delay(TimeSpan.FromSeconds(limitRemaining)).ConfigureAwait(false);
                        }
                    }
                }

                if (!foundLimit)
                {
                    await Task.Delay(TimeSpan.FromSeconds(60)).ConfigureAwait(false);
                }
            }
        }
    }

    private TimeLimiter GetBackendTimeLimiter()
    {
        var accountLimit1 = new CountByIntervalAwaitableConstraint(29, TimeSpan.FromSeconds(60));
        var accountLimit2 = new CountByIntervalAwaitableConstraint(99, TimeSpan.FromSeconds(1800));
        var ipLimit1 = new CountByIntervalAwaitableConstraint(44, TimeSpan.FromSeconds(60));
        var ipLimit2 = new CountByIntervalAwaitableConstraint(179, TimeSpan.FromSeconds(1800));
        return TimeLimiter.Compose(accountLimit1, accountLimit2, ipLimit1, ipLimit2);
    }

    private TimeLimiter GetSearchTimeLimiter()
    {
        var accountLimit = new CountByIntervalAwaitableConstraint(2, TimeSpan.FromSeconds(5));
        var ipLimit1 = new CountByIntervalAwaitableConstraint(7, TimeSpan.FromSeconds(10));
        var ipLimit2 = new CountByIntervalAwaitableConstraint(14, TimeSpan.FromSeconds(60));
        var ipLimit3 = new CountByIntervalAwaitableConstraint(59, TimeSpan.FromSeconds(300));
        return TimeLimiter.Compose(accountLimit, ipLimit1, ipLimit2, ipLimit3);
    }

    private TimeLimiter GetFetchTimeLimiter()
    {
        var accountLimit = new CountByIntervalAwaitableConstraint(5, TimeSpan.FromSeconds(4));
        var ipLimit1 = new CountByIntervalAwaitableConstraint(11, TimeSpan.FromSeconds(4));
        var ipLimit2 = new CountByIntervalAwaitableConstraint(15, TimeSpan.FromSeconds(12));
        return TimeLimiter.Compose(accountLimit, ipLimit1, ipLimit2);
    }

    private TimeLimiter GetWhisperTimeLimiter()
    {
        var accountLimit1 = new CountByIntervalAwaitableConstraint(14, TimeSpan.FromSeconds(60));
        var accountLimit2 = new CountByIntervalAwaitableConstraint(74, TimeSpan.FromSeconds(600));
        var accountLimit3 = new CountByIntervalAwaitableConstraint(149, TimeSpan.FromSeconds(3600));
        var accountLimit4 = new CountByIntervalAwaitableConstraint(599, TimeSpan.FromSeconds(43200));
        return TimeLimiter.Compose(accountLimit1, accountLimit2, accountLimit3, accountLimit4);
    }

    private TimeLimiter GetLeaguesViewLimiter()
    {
        var accountLimit1 = new CountByIntervalAwaitableConstraint(10, TimeSpan.FromSeconds(5));
        var accountLimit2 = new CountByIntervalAwaitableConstraint(20, TimeSpan.FromSeconds(10));
        var accountLimit3 = new CountByIntervalAwaitableConstraint(30, TimeSpan.FromSeconds(10));
        var ipLimit1 = new CountByIntervalAwaitableConstraint(20, TimeSpan.FromSeconds(5));
        var ipLimit2 = new CountByIntervalAwaitableConstraint(40, TimeSpan.FromSeconds(10));
        var ipLimit3 = new CountByIntervalAwaitableConstraint(60, TimeSpan.FromSeconds(10));
        return TimeLimiter.Compose(accountLimit1, accountLimit2, accountLimit3, ipLimit1, ipLimit2, ipLimit3);
    }

    private TimeLimiter GetExchangeLimiter()
    {
        var accountLimit1 = new CountByIntervalAwaitableConstraint(2, TimeSpan.FromSeconds(5));
        var ipLimit1 = new CountByIntervalAwaitableConstraint(6, TimeSpan.FromSeconds(15));
        var ipLimit2 = new CountByIntervalAwaitableConstraint(14, TimeSpan.FromSeconds(90));
        var ipLimit3 = new CountByIntervalAwaitableConstraint(44, TimeSpan.FromSeconds(300));
        return TimeLimiter.Compose(accountLimit1, ipLimit1, ipLimit2, ipLimit3);
    }
}