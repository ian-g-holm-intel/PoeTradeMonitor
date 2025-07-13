using Nito.AsyncEx;
using PoeLib.Common;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PoeLib.Settings;

public interface ISettingsManager
{
    PoeSettings Settings { get; }
    Task SaveAsync();
    void Save();
}

public class SettingsManager : ISettingsManager
{
    private static readonly string settingsFile = Path.Combine(Constants.DataDirectory, "settings.json");
    private readonly AsyncLock asyncLock = new();
    public PoeSettings Settings { get; private set; }

    public SettingsManager(PoeSettings settings)
    {
        Settings = settings;
    }

    public async Task SaveAsync()
    {
        using (await asyncLock.LockAsync())
        {
            Directory.CreateDirectory(Constants.DataDirectory);
            using var fileStream = File.Open(settingsFile, FileMode.Create);
            await JsonSerializer.SerializeAsync(fileStream, Settings, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public void Save()
    {
        using (asyncLock.Lock())
        {
            Directory.CreateDirectory(Constants.DataDirectory);
            using var fileStream = File.Open(settingsFile, FileMode.Create);
            JsonSerializer.Serialize(fileStream, Settings, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}