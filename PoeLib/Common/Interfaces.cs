using PoeTrade.Contracts;

namespace PoeLib.Common;

/// <summary>
/// Interface for objects that can be initialized.
/// </summary>
public interface IInitializable
{
    void Initialize(CancellationToken ct = default);
}

/// <summary>
/// Interface for push notification services.
/// </summary>
public interface INotificationClient
{
    void RegisterDeviceToken(string token);
    void ResetBadgeCount();
    Task SendPushNotification(string title, string subtitle, string body, string sound = "keys.caf");
}

/// <summary>
/// Interface for fetching Path of Exile item and currency prices from external sources.
/// </summary>
public interface IPriceFetcher
{
    string Name { get; }
    Task<Dictionary<TradeCurrencyType, CurrencyPrice>> GetCurrencyData(string league);
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

/// <summary>
/// Delegate for party join events.
/// </summary>
/// <param name="partyMembers">Array of party member names.</param>
public delegate void JoinedParty(string[] partyMembers);
