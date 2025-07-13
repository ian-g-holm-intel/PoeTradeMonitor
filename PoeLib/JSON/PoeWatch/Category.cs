using System.Collections.Generic;

namespace PoeLib.JSON.PoeWatch;

public class CategoryGroup
{
    public int id { get; set; }
    public string name { get; set; }
    public string display { get; set; }

    public override string ToString()
    {
        return name;
    }
}

public class Category
{
    public string name { get; set; }
    public string display { get; set; }
    public List<CategoryGroup> groups { get; set; }

    public override string ToString()
    {
        return name;
    }
}
