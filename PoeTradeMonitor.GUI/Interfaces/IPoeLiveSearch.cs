namespace PoeTradeMonitor.GUI.Interfaces;

public interface IPoeLiveSearch
{
    Task StartAsync(string league);
    Task StopAsync(CancellationToken ct = default);
    bool IsAlive { get; }
    bool Running { get; }
}
