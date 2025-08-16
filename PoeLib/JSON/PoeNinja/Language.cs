namespace PoeLib.JSON.PoeNinja;

public class Language
{
    public string name { get; set; } = string.Empty;
    public Translations translations { get; set; } = new Translations();
}

public class Translations
{
}
