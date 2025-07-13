using System.Net;
using System.Threading.Tasks;
using SpeedTest.Net.Enums;
using SpeedTest.Net.Models;

namespace SpeedTest.Net;

public class SpeedTestClient
{
    private readonly SpeedTestHttpClient speedTestHttpClient;
    public SpeedTestClient()
    {
        speedTestHttpClient = new SpeedTestHttpClient();
    }

    public SpeedTestClient(IWebProxy proxy)
    {
        speedTestHttpClient = new SpeedTestHttpClient(proxy);
    }

    /// <summary>
    /// Calculates download speed using the provided server
    /// </summary>
    /// <param name="server">The server object used for downloading files</param>
    /// <param name="unit">Unit in which Speed Test response should be returned</param>
    /// <returns>An instance of type DownloadSpeed</returns>
    public async Task<DownloadSpeed> GetDownloadSpeed(Server server = null, SpeedTestUnit unit = SpeedTestUnit.KiloBytesPerSecond) => await speedTestHttpClient.GetDownloadSpeed(server, unit);

    /// <summary>
    /// Finds the closest server to the provided co-ordinates
    /// </summary>
    /// <param name="latitude">Latitude of the location</param>
    /// <param name="longitude">Longitude of the location</param>
    /// <returns>An instance of type Server close to the provided latitude and longitude</returns>
    public async Task<Server> GetServer(double latitude, double longitude) => await speedTestHttpClient.GetServer(latitude, longitude);

    /// <summary>
    /// Finds the best server based on the callee location
    /// </summary>
    /// <returns>An instance of type Server close to the callee location</returns>
    public async Task<Server> GetServer() => await speedTestHttpClient.GetServer();

    /// <summary>
    /// Finds the best server based on the callee location
    /// </summary>
    /// <param name="serverId">SpeedTest.Net server ID</param>
    /// <returns>An instance of type Server close to the callee location</returns>
    public async Task<Server> GetServer(int serverId) => await speedTestHttpClient.GetServer(serverId);
}