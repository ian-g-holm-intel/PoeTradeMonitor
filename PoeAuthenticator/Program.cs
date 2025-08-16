using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PoeAuthenticator.Services;
using System.Net;
using System.Text;

namespace PoeAuthenticator;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection().AddLogging(builder => builder.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        })).AddPoeAuthenticator();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var poeCookieReader = serviceProvider.GetRequiredService<IPoeCookieReader>();
        var poeCookies = await poeCookieReader.GetPoeCookiesAsync(CancellationToken.None);
        var cookieContainer = serviceProvider.GetRequiredService<CookieContainer>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        cookieContainer.UpdateCookies(poeCookies, logger);
        var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("PoeApi");

        //var httpCLientHandler = new HttpClientHandler
        //{
        //    CookieContainer = cookieContainer,
        //    UseCookies = true,
        //    AllowAutoRedirect = false,
        //    AutomaticDecompression = DecompressionMethods.All
        //};
        //var httpClient = new HttpClient(httpCLientHandler);
        //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36");
        //httpClient.DefaultRequestHeaders.Add("Origin", "https://www.pathofexile.com");
        //httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
        //httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.6");
        //httpClient.DefaultRequestHeaders.Add("Priority", "u=1, i");
        //httpClient.BaseAddress = new Uri("https://www.pathofexile.com");
        var json = "{\"query\":{\"status\":{\"option\":\"online\"},\"stats\":[{\"type\":\"and\",\"filters\":[],\"disabled\":false}],\"filters\":{\"misc_filters\":{\"filters\":{\"corrupted\":{\"option\":\"true\"},\"gem_sockets\":{\"min\":5,\"max\":null}},\"disabled\":false},\"type_filters\":{\"filters\":{\"category\":{\"option\":\"gem.activegem\"},\"quality\":{\"min\":20}},\"disabled\":false},\"trade_filters\":{\"filters\":{\"price\":{\"min\":null,\"max\":4000,\"option\":null}},\"disabled\":false}}},\"sort\":{\"price\":\"asc\"}}";        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("https://www.pathofexile.com/api/trade2/search/poe2/Standard", content);
        if (response.IsSuccessStatusCode)
        {
            var contentResult = await response.Content.ReadAsStringAsync();
            Console.WriteLine(contentResult);
        }
    }
}