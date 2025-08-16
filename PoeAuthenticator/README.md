# PoeAuthenticator

Authentication and cookie management utility for Path of Exile accounts, providing secure session handling and automated login capabilities for the PoeTradeMonitor ecosystem.

## 🎯 Purpose

PoeAuthenticator handles the complex authentication requirements for Path of Exile services:
- **Session Management**: Maintain valid authentication sessions
- **Cookie Extraction**: Extract authentication cookies from browsers
- **Clearance Handling**: Manage Cloudflare protection bypass
- **Rate Limit Management**: Coordinate API usage across services
- **Secure Storage**: Safely store and manage credentials

## 🔐 Security Features

### Cookie Management
- **Browser Integration**: Extract cookies from Chrome, Firefox, Edge
- **Secure Storage**: Encrypted cookie storage using Windows DPAPI
- **Automatic Refresh**: Detect and refresh expired sessions
- **Multiple Accounts**: Support for multiple POE accounts

### Cloudflare Bypass
- **Clearance Tokens**: Automatic clearance token management
- **User-Agent Rotation**: Mimic legitimate browser requests
- **Rate Limiting**: Respect Cloudflare rate limits
- **Retry Logic**: Intelligent retry with exponential backoff

## 🏗️ Architecture

### Core Services

```csharp
public class PoeCookieReader
{
    public async Task<List<Cookie>> ExtractCookiesAsync(BrowserType browser)
    {
        return browser switch
        {
            BrowserType.Chrome => await ExtractChromeCookies(),
            BrowserType.Firefox => await ExtractFirefoxCookies(),
            BrowserType.Edge => await ExtractEdgeCookies(),
            _ => throw new NotSupportedException($"Browser {browser} not supported")
        };
    }
    
    private async Task<List<Cookie>> ExtractChromeCookies()
    {
        var cookiePath = GetChromeCookiePath();
        var database = new ChromeCookieDatabase(cookiePath);
        return await database.ReadCookiesAsync("pathofexile.com");
    }
}
```

### Authentication Flow

```csharp
public class PoeRateLimitService
{
    private readonly Dictionary<string, RateLimitState> _rateLimits = new();
    
    public async Task<bool> CanMakeRequestAsync(string endpoint)
    {
        var state = _rateLimits.GetOrCreate(endpoint);
        
        if (state.IsRateLimited)
        {
            var timeRemaining = state.ResetTime - DateTime.UtcNow;
            if (timeRemaining > TimeSpan.Zero)
            {
                await Task.Delay(timeRemaining);
            }
        }
        
        return true;
    }
    
    public void UpdateRateLimit(string endpoint, HttpResponseMessage response)
    {
        var state = _rateLimits.GetOrCreate(endpoint);
        
        if (response.Headers.TryGetValues("X-Rate-Limit-Remaining", out var remaining))
        {
            state.RemainingRequests = int.Parse(remaining.First());
        }
        
        if (response.Headers.TryGetValues("X-Rate-Limit-Reset", out var reset))
        {
            state.ResetTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset.First())).DateTime;
        }
    }
}
```

## 🌐 Browser Integration

### Chrome Cookie Extraction

```csharp
public class ChromeCookiesView
{
    public static List<XmlCookie> ExtractCookies(string domain)
    {
        var cookiePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            @"Google\Chrome\User Data\Default\Cookies");
            
        if (!File.Exists(cookiePath))
            throw new FileNotFoundException("Chrome cookie database not found");
            
        using var connection = new SqliteConnection($"Data Source={cookiePath}");
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT host_key, name, value, encrypted_value, expires_utc, is_secure, is_httponly
            FROM cookies 
            WHERE host_key LIKE @domain";
        command.Parameters.AddWithValue("@domain", $"%{domain}%");
        
        var cookies = new List<XmlCookie>();
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var cookie = new XmlCookie
            {
                Domain = reader.GetString("host_key"),
                Name = reader.GetString("name"),
                Value = DecryptCookieValue(reader["encrypted_value"] as byte[]),
                Expires = ConvertChromeTime(reader.GetInt64("expires_utc")),
                Secure = reader.GetBoolean("is_secure"),
                HttpOnly = reader.GetBoolean("is_httponly")
            };
            
            cookies.Add(cookie);
        }
        
        return cookies;
    }
}
```

### Firefox Cookie Support

```csharp
public class FirefoxCookieReader
{
    public static List<XmlCookie> ExtractFirefoxCookies(string domain)
    {
        var profilePath = GetFirefoxProfilePath();
        var cookiesPath = Path.Combine(profilePath, "cookies.sqlite");
        
        if (!File.Exists(cookiesPath))
            throw new FileNotFoundException("Firefox cookie database not found");
            
        // Create temporary copy to avoid locking issues
        var tempPath = Path.GetTempFileName();
        File.Copy(cookiesPath, tempPath, true);
        
        try
        {
            using var connection = new SqliteConnection($"Data Source={tempPath}");
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT host, name, value, expiry, isSecure, isHttpOnly
                FROM moz_cookies 
                WHERE host LIKE @domain";
            command.Parameters.AddWithValue("@domain", $"%{domain}%");
            
            // Extract cookies similar to Chrome implementation
            return ExtractCookiesFromReader(command.ExecuteReader());
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
```

## 🔄 Session Management

### Automatic Session Refresh

```csharp
public class SessionManager
{
    private readonly Timer _refreshTimer;
    private readonly List<AuthenticatedSession> _sessions = new();
    
    public SessionManager()
    {
        _refreshTimer = new Timer(CheckSessionHealth, null, 
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }
    
    private async void CheckSessionHealth(object state)
    {
        foreach (var session in _sessions.ToList())
        {
            if (await IsSessionExpired(session))
            {
                await RefreshSession(session);
            }
        }
    }
    
    private async Task<bool> IsSessionExpired(AuthenticatedSession session)
    {
        try
        {
            var response = await _httpClient.GetAsync("https://www.pathofexile.com/character-window/get-characters",
                CreateRequestWithCookies(session.Cookies));
                
            return response.StatusCode == HttpStatusCode.Unauthorized;
        }
        catch
        {
            return true; // Assume expired on error
        }
    }
}
```

### Cookie Encryption

```csharp
public class SecureCookieStorage
{
    public void StoreCookies(List<XmlCookie> cookies, string accountName)
    {
        var json = JsonSerializer.Serialize(cookies);
        var encrypted = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(json),
            GetEntropy(accountName),
            DataProtectionScope.CurrentUser);
            
        var cookiePath = GetCookieStoragePath(accountName);
        File.WriteAllBytes(cookiePath, encrypted);
    }
    
    public List<XmlCookie> LoadCookies(string accountName)
    {
        var cookiePath = GetCookieStoragePath(accountName);
        if (!File.Exists(cookiePath))
            return new List<XmlCookie>();
            
        var encrypted = File.ReadAllBytes(cookiePath);
        var decrypted = ProtectedData.Unprotect(
            encrypted,
            GetEntropy(accountName),
            DataProtectionScope.CurrentUser);
            
        var json = Encoding.UTF8.GetString(decrypted);
        return JsonSerializer.Deserialize<List<XmlCookie>>(json);
    }
}
```

## 🛡️ Security Measures

### Credential Protection

```csharp
public class CredentialManager
{
    private readonly string _credentialTarget = "PoeTradeMonitor";
    
    public void StoreCredentials(string username, string password)
    {
        var credential = new Credential
        {
            Target = _credentialTarget,
            Username = username,
            Password = password,
            Persist = CredentialPersistence.LocalMachine
        };
        
        credential.Save();
    }
    
    public (string username, string password) LoadCredentials()
    {
        var credential = Credential.Load(_credentialTarget);
        return credential != null 
            ? (credential.Username, credential.Password)
            : (null, null);
    }
}
```

### Anti-Detection Measures

```csharp
public class AntiDetectionHandler : DelegatingHandler
{
    private readonly Random _random = new();
    private readonly string[] _userAgents = 
    {
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:121.0) Gecko/20100101 Firefox/121.0",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 Edg/120.0.0.0"
    };
    
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Add realistic headers
        request.Headers.Add("User-Agent", _userAgents[_random.Next(_userAgents.Length)]);
        request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        request.Headers.Add("DNT", "1");
        request.Headers.Add("Connection", "keep-alive");
        request.Headers.Add("Upgrade-Insecure-Requests", "1");
        
        // Random delay to appear more human
        await Task.Delay(_random.Next(100, 500), cancellationToken);
        
        return await base.SendAsync(request, cancellationToken);
    }
}
```

## 🚀 Usage Examples

### Basic Authentication

```csharp
// Extract cookies from browser
var cookieReader = new PoeCookieReader();
var cookies = await cookieReader.ExtractCookiesAsync(BrowserType.Chrome);

// Store securely
var storage = new SecureCookieStorage();
storage.StoreCookies(cookies, "MyAccount");

// Create authenticated HTTP client
var httpClient = new HttpClient(new AntiDetectionHandler());
foreach (var cookie in cookies)
{
    httpClient.DefaultRequestHeaders.Add("Cookie", $"{cookie.Name}={cookie.Value}");
}
```

### Session Validation

```csharp
// Check if session is still valid
var validator = new SessionValidator();
var isValid = await validator.ValidateSessionAsync(cookies);

if (!isValid)
{
    // Attempt to refresh
    var refreshed = await validator.RefreshSessionAsync();
    if (refreshed)
    {
        cookies = await cookieReader.ExtractCookiesAsync(BrowserType.Chrome);
    }
}
```

### Rate Limit Management

```csharp
// Coordinate rate limits across services
var rateLimitService = new PoeRateLimitService();

await rateLimitService.CanMakeRequestAsync("/api/trade/search");
var response = await httpClient.GetAsync("https://www.pathofexile.com/api/trade/search/Standard");
rateLimitService.UpdateRateLimit("/api/trade/search", response);
```

## 📁 Project Structure

```
PoeAuthenticator/
├── Services/              # Core authentication services
│   ├── AlphaVssService.cs
│   ├── CookieMonitorService.cs
│   └── PoeCookieReader.cs
├── Schema/               # Data models
│   ├── CookiesList.cs
│   └── XmlCookie.cs
├── Bootstrapper.cs       # Dependency injection setup
├── ClearanceHandler.cs   # Cloudflare bypass
├── LoginDelegateHandler.cs # Authentication flow
├── PoeRateLimitHandler.cs # Rate limiting
├── PoeRateLimitService.cs # Rate limit coordination
└── Program.cs           # Console application entry
```

## 🧪 Testing

```bash
# Run authentication tests
dotnet test PoeAuthenticator.Tests/

# Test cookie extraction
dotnet test --filter "CookieExtractionTests"

# Test rate limiting
dotnet test --filter "RateLimitTests"
```

## ⚙️ Configuration

### Application Settings

```json
{
  "Authentication": {
    "DefaultBrowser": "Chrome",
    "CookieRefreshInterval": "00:05:00",
    "SessionValidationInterval": "00:15:00",
    "MaxRetryAttempts": 3
  },
  "RateLimit": {
    "RequestsPerMinute": 45,
    "BurstLimit": 10,
    "BackoffMultiplier": 2.0,
    "MaxBackoffSeconds": 300
  },
  "Security": {
    "EncryptCookies": true,
    "UseAntiDetection": true,
    "RotateUserAgents": true,
    "RandomDelayMin": 100,
    "RandomDelayMax": 500
  }
}
```

## 🔍 Troubleshooting

### Common Issues

**Cookies Not Found**
- Ensure browser is closed before extraction
- Check browser profile location
- Verify domain matches exactly

**Authentication Fails**
- Clear browser cache and cookies
- Log in manually to POE website first
- Check for two-factor authentication

**Rate Limiting**
- Reduce request frequency
- Implement proper backoff strategy
- Use multiple IP addresses if possible

## 📋 Dependencies

### Required Packages
- **System.Data.SQLite**: Browser database access
- **System.Security.Cryptography**: Cookie encryption
- **Microsoft.Extensions.DependencyInjection**: Service container
- **Microsoft.Extensions.Hosting**: Background services
- **System.Text.Json**: Configuration serialization

### External Tools
- **ChromeCookiesView.exe**: Chrome cookie extraction utility
- **AlphaVSS**: Volume shadow copy service for locked files

## 🤝 Contributing

When contributing to PoeAuthenticator:

1. **Security First**: Never log or expose sensitive data
2. **Browser Compatibility**: Test with multiple browser versions
3. **Error Handling**: Implement comprehensive error recovery
4. **Rate Limiting**: Respect all API rate limits
5. **Documentation**: Document security considerations

## 🔗 Related Projects

- [PoeLib](../PoeLib/) - Core POE integration library
- [PoeTradeMonitor.Service](../PoeTradeMonitor.Service/) - Background service
- [PoeTradeMonitor.GUI](../PoeTradeMonitor.GUI/) - Desktop application
- [PoeTrade.Contracts](../PoeTrade.Contracts/) - Shared data contracts