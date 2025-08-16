using Nito.AsyncEx;
using PoeLib.Common;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;

namespace PoeTradeMonitor.GUI.Settings;

public class SettingsManager
{
    private readonly string settingsFile;
    private readonly AsyncLock asyncLock = new();
    private readonly IFileSystem fileSystem;
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true
    };
    private readonly PoeSettings settings;

    public SettingsManager(PoeSettings settings, IFileSystem? fileSystem = null)
    {
        this.settings = settings;
        this.fileSystem = fileSystem ?? new FileSystem();
        this.settingsFile = this.fileSystem.Path.Combine(Constants.DataDirectory, "settings.json");
        this.settings.PropertyChanged += OnSettingsPropertyChanged;
    }

    private void OnSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Use fire-and-forget async to save immediately without blocking the UI
        _ = Task.Run(async () =>
        {
            try
            {
                await SaveAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        });
    }

    private async Task SaveAsync()
    {
        using (await asyncLock.LockAsync())
        {
            fileSystem.Directory.CreateDirectory(Constants.DataDirectory);
            using var fileStream = fileSystem.File.Open(settingsFile, FileMode.Create);
            await JsonSerializer.SerializeAsync(fileStream, settings, JsonOptions);
        }
    }
}