using Microsoft.Extensions.Logging;
using System.Net;

namespace PoeAuthenticator.Services;

public interface ICookieMonitorService
{
    event Action? CookiesUpdated;
    void Start();
    void Stop();
}

public class CookieMonitorService : ICookieMonitorService, IDisposable
{
    private readonly FileSystemWatcher cookieWatcher;
    private readonly ILogger<CookieMonitorService> logger;
    private readonly IPoeCookieReader poeCookieReader;
    private readonly CookieContainer cookieContainer;
    private bool disposedValue;

    public event Action? CookiesUpdated;

    public CookieMonitorService(ILogger<CookieMonitorService> logger, IPoeCookieReader poeCookieReader, CookieContainer cookieContainer)
    {
        this.logger = logger;
        this.poeCookieReader = poeCookieReader;
        this.cookieContainer = cookieContainer;

        var cookiesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BraveSoftware", "Brave-Browser", "User Data", "Profile 1", "Network", "Cookies");
        cookieWatcher = new FileSystemWatcher
        {
            Path = Path.GetDirectoryName(cookiesPath)!,
            Filter = Path.GetFileName(cookiesPath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime
        };
        cookieWatcher.Changed += OnCookiesChanged;
    }

    public void Start()
    {
        cookieWatcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        cookieWatcher.EnableRaisingEvents = false;
    }

    private void OnCookiesChanged(object sender, FileSystemEventArgs e)
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            var poeCookies = await poeCookieReader.GetPoeCookiesAsync(CancellationToken.None);
            cookieContainer.UpdateCookies(poeCookies, logger);
            CookiesUpdated?.Invoke();
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                cookieWatcher.EnableRaisingEvents = false;
                cookieWatcher.Changed -= OnCookiesChanged;
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
