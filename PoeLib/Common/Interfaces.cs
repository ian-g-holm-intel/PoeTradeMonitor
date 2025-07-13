using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using PoeLib.JSON;

namespace PoeLib;

public interface IInitializable
{
    void Initialize(CancellationToken ct = default);
}

public interface IProxyRetriever
{
    Task<Dictionary<string, WebProxy>> GetProxiesAsync();
}

public interface IPoePriceChecker
{
    void Start();
    void Stop();
    decimal GetPrice(string itemName);
    Task UpdatePricesAndLogAsync();
}

public interface INotificationClient
{
    void RegisterDeviceToken(string token);
    void ResetBadgeCount();
    Task SendPushNotification(string title, string subtitle, string body, string sound = "keys.caf");
}

public interface IPriceFetcher
{
    string Name { get; }
    Task<Dictionary<CurrencyType, CurrencyPrice>> GetCurrencyData(string league);
    Task<SearchItemGroup> GetFragmentData(string league);
    Task<SearchItemGroup> GetDivinationCardData(string league);
    Task<SearchItemGroup> GetUniqueMapData(string league);
    Task<SearchItemGroup> GetUniqueJewelData(string league);
    Task<SearchItemGroup> GetUniqueFlaskData(string league);
    Task<SearchItemGroup> GetUniqueWeaponData(string league);
    Task<SearchItemGroup> GetUniqueArmorData(string league);
    Task<SearchItemGroup> GetUniqueAccessoryData(string league);
    Task<SearchItemGroup> GetGemsData(string league);
    Task<SearchItemGroup> GetFossils(string league);
}

public interface IPoeLiveSearch
{
    Task StartAsync(string league);
    Task StopAsync(CancellationToken ct = default);
    bool IsAlive { get; }
    bool Running { get; }
}

public delegate void JoinedParty(string[] partyMembers);
