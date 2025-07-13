using CliWrap;
using PoeAuthenticator.Schema;
using System.Xml.Serialization;

namespace PoeAuthenticator.Services;

public interface IPoeCookieReader
{
    Task<Dictionary<string, string>> GetPoeCookiesAsync(CancellationToken cancellationToken);
}

public class PoeCookieReader : IPoeCookieReader, IDisposable
{
    private readonly IAlphaVssService alphaVssService;
    private readonly FileSystemWatcher cookieWatcher;
    private static readonly object syncLock = new();
    private bool disposedValue;

    private string localAppDataPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private string cookiesPath => Path.Combine(localAppDataPath, "BraveSoftware", "Brave-Browser", "User Data", "Profile 1", "Network", "Cookies");
    

    public PoeCookieReader(IAlphaVssService alphaVssService)
    {
        this.alphaVssService = alphaVssService;
    }

    public async Task<Dictionary<string, string>> GetPoeCookiesAsync(CancellationToken cancellationToken)
    {
        var cookiesPath = GetCookiesPath();
        var cookiesXmlPath = await GetCookiesXmlAsync(cookiesPath, cancellationToken).ConfigureAwait(false);
        var cookiesList = await DeserializeCookiesAsync(cookiesXmlPath, cancellationToken).ConfigureAwait(false);
        return cookiesList.Items
            .Where(cookie => cookie.host_name.Equals(".pathofexile.com", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                cookie => cookie.name,
                cookie => cookie.value,
                StringComparer.OrdinalIgnoreCase
            );
    }

    public static async Task<CookiesList> DeserializeCookiesAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(CookiesList));
            using (var streamReader = new StreamReader(filePath))
            {
                string xmlContent = await streamReader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
                using (var stringReader = new StringReader(xmlContent))
                {
                    return (CookiesList)serializer.Deserialize(stringReader);
                }
            }
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"Cookie XML file not found at path: {filePath}");
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"Error deserializing cookie XML: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Unexpected error while deserializing cookie XML: {ex.Message}", ex);
        }
    }

    private async Task<string> GetCookiesXmlAsync(string cookieFilePath, CancellationToken cancellationToken)
    {
        var outputPath = Path.Combine(Path.GetDirectoryName(cookieFilePath)!, Path.GetFileNameWithoutExtension(cookieFilePath) + ".xml");
        await Cli.Wrap(Environment.GetEnvironmentVariable("CHROME_COOKIES_VIEW_PATH") ?? Path.Combine(AppContext.BaseDirectory, "ChromeCookiesView.exe"))
            .WithArguments($"/CookiesFile \"{cookieFilePath}\" /sxml \"{outputPath}\"")
            .ExecuteAsync(cancellationToken).ConfigureAwait(false);
        return outputPath;
    }

    private string GetCookiesPath()
    {
        if (!File.Exists(cookiesPath))
            throw new FileNotFoundException(cookiesPath);
        string backupCookiesPath = Path.Combine(localAppDataPath, "BraveSoftware", "Brave-Browser", "User Data", "Profile 1", "Network", "Cookies-Backup");
        try
        {
            File.Copy(cookiesPath, backupCookiesPath, true);
        }
        catch
        {
            lock (syncLock)
            {
                alphaVssService.ShadowCopyFile(cookiesPath, backupCookiesPath);
            }
        }
        return backupCookiesPath;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                cookieWatcher?.Dispose();
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