using SpeedTest.Net;
using System.Net;
using System.Threading.Tasks;

namespace PoeLib.Proxies;

public interface IProxySpeedTester
{
    Task<double> GetDownloadSpeed(IWebProxy proxy);
}

public class ProxySpeedTester : IProxySpeedTester
{
    public async Task<double> GetDownloadSpeed(IWebProxy proxy)
    {
        var speedTestClient = new SpeedTestClient(proxy);
        var speedTestServer = await speedTestClient.GetServer(29938);
        var result = await speedTestClient.GetDownloadSpeed(speedTestServer, SpeedTest.Net.Enums.SpeedTestUnit.MegaBitsPerSecond);
        return result.Speed;
    }
}
