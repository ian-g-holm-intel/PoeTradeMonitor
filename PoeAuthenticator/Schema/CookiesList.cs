using System.Xml.Serialization;

namespace PoeAuthenticator.Schema;

[XmlRoot("cookies_list")]
public class CookiesList
{
    [XmlElement("item")]
    public List<XmlCookie> Items { get; set; }
}