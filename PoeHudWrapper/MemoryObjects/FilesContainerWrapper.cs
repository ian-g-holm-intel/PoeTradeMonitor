using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Static;

namespace PoeHudWrapper.MemoryObjects;

public partial class FilesContainerWrapper : IDisposable
{
    private readonly IMemory _memory;
    private readonly CoreSettings _settings;
    public readonly FilesFromMemory FilesFromMemory;
    private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();

    public FilesContainerWrapper(IMemory memory, CoreSettings settings)
    {
        _memory = memory;
        _settings = settings;
        ItemClasses = new ItemClasses();
        FilesFromMemory = new FilesFromMemory(_memory);
        AllFiles = FilesFromMemory.GetAllFiles();

        Task.Run(() =>
        {
            ParseFiles(AllFiles);
        });
        Task.Run(async () =>
        {
            while (!_cancellation.IsCancellationRequested)
            {
                try
                {
                    if (settings.FileReloadPeriod <= 0 || Core.Current?.IsForeground != true)
                    {
                        await Task.Delay(1000, _cancellation.Token).ContinueWith(t => { });
                    }

                    await Task.Delay(settings.FileReloadPeriod, _cancellation.Token).ContinueWith(t => { });
                    if (!_cancellation.IsCancellationRequested)
                    {
                        LoadFiles(false);
                    }
                }
                catch (Exception ex)
                {
                    DebugWindow.LogError(ex.ToString());
                }
            }
        });
    }

    public Dictionary<string, FileInformation> AllFiles { get; private set; }
    public Dictionary<string, FileInformation> Metadata { get; } = new Dictionary<string, FileInformation>();
    public Dictionary<string, FileInformation> Data { get; private set; } = new Dictionary<string, FileInformation>();
    public Dictionary<string, FileInformation> OtherFiles { get; } = new Dictionary<string, FileInformation>();

    public void LoadFiles() => LoadFiles(true);

    private void LoadFiles(bool log)
    {
        var newAllFiles = FilesFromMemory.GetAllFiles(log);
        AllFiles = newAllFiles;
    }

    public void ParseFiles(Dictionary<string, FileInformation> files)
    {
        foreach (var file in files)
        {
            if (string.IsNullOrEmpty(file.Key))
                continue;

            if (file.Key.StartsWith("Metadata/", StringComparison.Ordinal))
                Metadata[file.Key] = file.Value;
            else if (file.Key.StartsWith("Data/", StringComparison.Ordinal) && file.Key.EndsWith(".dat", StringComparison.Ordinal))
                Data[file.Key] = file.Value;
            else
                OtherFiles[file.Key] = file.Value;
        }
    }

    public void ParseFiles()
    {
        if (AllFiles != null)
        {
            ParseFiles(AllFiles);
        }
    }

    public long FindFile(string name)
    {
        return AllFiles.TryGetValue(name, out var result)
            ? result.Ptr
            : 0;
    }

    public void Dispose()
    {
        _cancellation.Cancel();
    }
}