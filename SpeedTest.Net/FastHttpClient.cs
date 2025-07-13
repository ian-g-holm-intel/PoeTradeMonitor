﻿using Newtonsoft.Json;
using SpeedTest.Net.Enums;
using SpeedTest.Net.Helpers;
using SpeedTest.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpeedTest.Net;

internal class FastHttpClient : BaseHttpClient
{
    public FastHttpClient() : base(new HttpClientHandler() { Proxy = new WebProxy() })
    {
        DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
        Timeout = TimeSpan.FromSeconds(5);
    }

    public FastHttpClient(WebProxy proxy) : base(new HttpClientHandler() { Proxy = proxy })
    {
        DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
        Timeout = TimeSpan.FromSeconds(5);
    }

    private readonly string Api = "https://api.fast.com/netflix/speedtest/";
    private readonly string Website = "https://fast.com/";
    private readonly string TokenIdentifier = "token:";

    private readonly string KeyHttps = "https";
    private readonly string KeyUrlCount = "urlCount";   
    private readonly string KeyToken = "token";

    private readonly bool ValueHttps = true;
    private readonly int ValueUrlCount = 5;

    private static string Token { get; set; }

    internal async Task<DownloadSpeed> GetDownloadSpeed(SpeedTestUnit unit = SpeedTestUnit.KiloBytesPerSecond)
    {
        if (string.IsNullOrEmpty(Token))
        {
            var jsonFilePath = await GetJsonFilePath();
            if(string.IsNullOrEmpty(jsonFilePath))
                return new DownloadSpeed() { Speed = 0, Unit = unit.ToString(), Source = SpeedTestSource.Fast.ToSourceString() };

            Token = await GetToken(jsonFilePath);
        }

        var urls = await GetUrls(Token);

        var speed = await GetDownloadSpeed(urls?.Select(x => x.Url.AbsoluteUri), unit);

        return new DownloadSpeed
        {
            Speed = speed.Speed,
            Unit = speed.Unit,
            Source = SpeedTestSource.Fast.ToSourceString()
        };
    }

    private async Task<List<FileUrl>> GetUrls(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var urls = await GetStringAsync($"{Api}?{KeyHttps}={ValueHttps}&{KeyUrlCount}={ValueUrlCount}&{KeyToken}={token}");

            return JsonConvert.DeserializeObject<List<FileUrl>>(urls);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<string> GetToken(string jsFilePath)
    {
        try
        {
            if (string.IsNullOrEmpty(jsFilePath))
                return "";

            var javascript = await GetStringAsync(jsFilePath);

            int? index = javascript?.IndexOf(TokenIdentifier);
            if (index == null || index == -1)
                return "";

            javascript = javascript.Substring(index ?? 0);

            index = javascript?.IndexOf(",");
            if (index == null || index == -1)
                return "";

            javascript = javascript.Substring(0, index ?? 0);

            return javascript.Replace("\"", "").Replace(TokenIdentifier, "");
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> GetJsonFilePath()
    {
        try
        {
            var html = await GetStringAsync(Website);

            int? index = html.IndexOf("<script src=");
            if (index == null || index == -1)
                return null;

            html = html.Substring((index ?? 0) + 13);
            var jsFileName = html.Substring(0, html.IndexOf("\""));
            return Website + jsFileName;
        }
        catch(Exception)
        {
            return null;
        }
    }
}