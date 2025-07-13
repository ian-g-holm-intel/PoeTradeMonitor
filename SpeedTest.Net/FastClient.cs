using SpeedTest.Net.Enums;
using SpeedTest.Net.Models;
using System.Net;
using System.Threading.Tasks;

namespace SpeedTest.Net;

public class FastClient
{
    private readonly FastHttpClient fastHttpClient;
    public FastClient()
    {
        fastHttpClient = new FastHttpClient();
    }

    public FastClient(WebProxy proxy)
    {
        fastHttpClient = new FastHttpClient(proxy);
    }

    /// <summary>
    /// Calculates download speed using the provided server
    /// </summary>
    /// <param name="unit">Specifies in which unit download speed should be returned</param>
    /// <returns>An instance of type DownloadSpeed</returns>
    public async Task<DownloadSpeed> GetDownloadSpeed(SpeedTestUnit unit = SpeedTestUnit.KiloBytesPerSecond) => await fastHttpClient?.GetDownloadSpeed(unit);
}