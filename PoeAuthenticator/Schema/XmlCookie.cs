using System.Xml.Serialization;

namespace PoeAuthenticator.Schema;

public class XmlCookie
{
    public string host_name { get; set; } = string.Empty;
    public string path { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string value { get; set; } = string.Empty;
    public string secure { get; set; } = string.Empty;
    public string http_only { get; set; } = string.Empty;

    private DateTime? _lastAccessed;
    private DateTime? _createdOn;
    private DateTime? _expires;

    [XmlElement("last_accessed")]
    public string LastAccessedString
    {
        get => _lastAccessed?.ToString("M/d/yyyy h:mm:ss tt") ?? string.Empty;
        set => _lastAccessed = string.IsNullOrEmpty(value) ? null : DateTime.Parse(value);
    }

    [XmlElement("created_on")]
    public string CreatedOnString
    {
        get => _createdOn?.ToString("M/d/yyyy h:mm:ss tt") ?? string.Empty;
        set => _createdOn = string.IsNullOrEmpty(value) ? null : DateTime.Parse(value);
    }

    [XmlElement("expires")]
    public string ExpiresString
    {
        get => _expires?.ToString("M/d/yyyy h:mm:ss tt") ?? string.Empty;
        set => _expires = string.IsNullOrEmpty(value) ? null : DateTime.Parse(value);
    }

    public string encryption_type { get; set; } = string.Empty;

    [XmlIgnore]
    public DateTime? LastAccessed => _lastAccessed;

    [XmlIgnore]
    public DateTime? CreatedOn => _createdOn;

    [XmlIgnore]
    public DateTime? Expires => _expires;
}