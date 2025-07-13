using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ComposableAsync;
using NUnit.Framework;
using RateLimiter;

namespace PoeLib.Tests;

[TestFixture]
public class RateLimiterTests
{
    private TimeLimiter textCommandLimiter;
    private TimeLimiter apiLimiter;

    [SetUp]
    public void Setup()
    {
        textCommandLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMilliseconds(200));
        apiLimiter = TimeLimiter.GetFromMaxCountByInterval(5, TimeSpan.FromSeconds(5));
    }

    [Test]
    public async Task SendTextCommand_CommandsNotLost()
    {
        var count = 0;
        var totalTime = Stopwatch.StartNew();
        for (int i = 0; i < 10; i++)
        {
            await textCommandLimiter;
            count++;
        }
        totalTime.Stop();
        Assert.Equals(10, count);
        Assert.That(totalTime.Elapsed >= TimeSpan.FromMilliseconds(1800));
    }

    [Test]
    public async Task SendAPI_CommandsNotLost()
    {
        var count = 0;
        var totalTime = Stopwatch.StartNew();
        for (int i = 0; i < 25; i++)
        {
            await apiLimiter;
            count++;
        }
        totalTime.Stop();
        Assert.Equals(25, count);
        Assert.That(totalTime.Elapsed >= TimeSpan.FromSeconds(20));
    }
}
