using Newtonsoft.Json;
using SpeedTest.Net.Enums;
using SpeedTest.Net.Helpers;
using SpeedTest.Net.LocalData;
using SpeedTest.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpeedTest.Net;

public class SpeedTestHttpClient : BaseHttpClient
{
    public SpeedTestHttpClient() : base(new HttpClientHandler() { Proxy = new WebProxy() })
    {
        DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
        Timeout = TimeSpan.FromSeconds(2);
    }

    public SpeedTestHttpClient(IWebProxy proxy) : base(new HttpClientHandler() { Proxy = proxy })
    {
        DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
        Timeout = TimeSpan.FromSeconds(2);
    }

    private static readonly ServersList ServersConfig = new ServersList(
        JsonConvert.DeserializeObject<List<Server>>(
            LocalDataHelper.ReadLocalFile("servers.json")
        )
    );

    private readonly int[] DownloadSizes = { 350 };

    private IEnumerable<string> GenerateDownloadUrls(Server server, int retryCount = 1)
    {
        var downloadUriBase = new Uri(new Uri(server.Url), ".").OriginalString + "random{0}x{0}.jpg?r={1}";

        foreach (var downloadSize in DownloadSizes)
        {
            for (var i = 0; i < retryCount; i++)
            {
                yield return string.Format(downloadUriBase, downloadSize, i + 1);
            }
        }
    }

    internal async Task<Server> GetServer(double latitude, double longitude)
    {
        try
        {
            return await Task.Factory.StartNew(() =>
            {
                var _server = new Server() { Latitude = latitude, Longitude = longitude };

                ServersConfig.CalculateDistances(_server.GeoCoordinate);

                return ServersConfig.Servers
                    .Where(s => !ServersConfig.IgnoreIds.Contains(s.Id))
                    .OrderBy(s => s.Distance)
                    .FirstOrDefault();
            });
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get Server based on the co-ordinates {latitude},{longitude}", ex);
        }
    }

    internal Task<Server> GetServer(int serverId)
    {
        return Task.FromResult(ServersConfig.Servers.Single(s => s.Id == serverId));
    }

    internal async Task<Server> GetServer(string ip = "")
    {
        try
        {
            var url = "https://ipinfo.io/json";

            if (!string.IsNullOrEmpty(ip?.Trim()))
                url = $"https://ipinfo.io/{ip}/json";

            var loc = JsonConvert.DeserializeObject<LocationModel>(await GetStringAsync("https://ipinfo.io/json"));
            return await GetServer(loc.Latitude, loc.Longitude);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get Server based on the callee location", ex);
        }
    }

    internal async Task<DownloadSpeed> GetDownloadSpeed(Server server = null, SpeedTestUnit unit = SpeedTestUnit.KiloBytesPerSecond)
    {
        try
        {
            if (server == null)
                server = await GetServer();

            if (string.IsNullOrEmpty(server?.Url?.Trim()))
                throw new Exception("Failed to get download speed");

            var downloadUrls = GenerateDownloadUrls(server, 1);

            if (downloadUrls?.Any() != true)
                throw new Exception("Couldn't fetch downloadable urls");

            var speed = await GetDownloadSpeed(downloadUrls, unit);

            return new DownloadSpeed
            {
                Server = server,
                Speed = speed.Speed,
                Unit = speed.Unit,
                Source = SpeedTestSource.Speedtest.ToSourceString()
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get download speed", ex);
        }
    }
}