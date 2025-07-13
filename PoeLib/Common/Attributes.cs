using System;
using System.Drawing;

namespace PoeLib;

public abstract class LocationAttribute : Attribute
{
    public Rectangle Screen { get; set; }

    protected LocationAttribute(Rectangle screen)
    {
        Screen = screen;
    }
}

public class Location2560x1440Attribute : LocationAttribute
{
    public Location2560x1440Attribute(int x, int y) : base(new Rectangle(x, y, 2560, 1440)) { }
}

public class Location1920x1080Attribute : LocationAttribute
{
    public Location1920x1080Attribute(int x, int y) : base(new Rectangle(x, y, 1920, 1080)) { }
}

public class Location1280x1024Attribute : LocationAttribute
{
    public Location1280x1024Attribute(int x, int y) : base(new Rectangle(x, y, 1280, 1024)) { }
}

public class StackSizeAttribute : Attribute
{
    public int Size { get; set; }

    public StackSizeAttribute(int size)
    {
        Size = size;
    }
}

public class TradeNameAttribute : Attribute
{
    public string Name { get; set; }

    public TradeNameAttribute(string name)
    {
        Name = name;
    }
}

public class SellingAttribute : Attribute
{
    public int StockSize { get; }

    public SellingAttribute(int stockSize)
    {
        StockSize = stockSize;
    }
}

public class BuyingAttribute : Attribute
{
    public int StockSize { get; }

    public BuyingAttribute(int stockSize)
    {
        StockSize = stockSize;
    }
}
