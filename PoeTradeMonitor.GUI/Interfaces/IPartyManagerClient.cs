namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPartyManagerClient
{
    Task<bool> Start(string accountName, string characterName, int value, string clientName);
    Task<bool> Stop(string clientName);
    Task LeaveParty(string clientName);
    Task<string[]> GetPartyMembers(string clientName);
}
