using System;
using System.Collections.Generic;

namespace PoeLib.Proxies;

public class Ports
{
    public int http { get; set; }
    public int socks5 { get; set; }
}

public class WebshareProxyServer
{
    public string username { get; set; }
    public string password { get; set; }
    public string proxy_address { get; set; }
    public Ports ports { get; set; }
    public bool valid { get; set; }
    public DateTime last_verification { get; set; }
    public string country_code { get; set; }
    public double country_code_confidence { get; set; }
    public double speed { get; set; }
}

public class WebshareProxyServers
{
    public int count { get; set; }
    public object next { get; set; }
    public object previous { get; set; }
    public List<WebshareProxyServer> results { get; set; }
}
