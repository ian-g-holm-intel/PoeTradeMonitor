namespace PoeLib.JSON.PoeWatch;

public class CategoryGroup
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string display { get; set; } = string.Empty;

    public override string ToString()
    {
        return name;
    }
}

public class Category
{
    public string name { get; set; } = string.Empty;
    public string display { get; set; } = string.Empty;
    public List<CategoryGroup> groups { get; set; } = new List<CategoryGroup>();

    public override string ToString()
    {
        return name;
    }
}
