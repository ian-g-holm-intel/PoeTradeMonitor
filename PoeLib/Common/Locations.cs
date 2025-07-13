using System.Drawing;

namespace PoeLib;

public interface ILocations
{
    int InventorySpaceSize { get; }
    int InventorySpaceQuadSize { get; }
    Point InventoryOffset { get; }
    Point ChaosSaleOffset { get; }
    Point Stash { get; }
    Point Waypoint { get; }
    Point Part1 { get; }
    Point Act1 { get; }
    Point Act1Town { get; }
    Point TradeAccept { get; }
    Point TradeRequestAccept { get; }
    Point CurrencyTab { get; }
    Point ChaosSaleTab { get; }
    Point EssenceTab { get; }
    Point SetPriceMenuOffset { get; }
    Point SetPriceMenuValueOffset { get; }
    Point SetPriceMenuClearOffset { get; }
    int SetPriceValueOffset { get; }
    Point SetPriceTypeOffset { get; }
    Point SetPriceTypeValueOffset { get; }
    Point SetPriceTypeValueBottomOffset { get; }
    Point TheirTradeOffset { get; }
    Point PartyTab { get; }
    Point ItemSaleTab { get; }
    Point PartyPortrait { get; }
    Point LeaveParty { get; }
    int ThresholdXOffset { get; set; }
    int NewXOffset { get; set; }
    int ThresholdYOffset { get; set; }
}

public class Locations2560x1440 : ILocations
{
    public int InventorySpaceSize { get; } = 71;
    public int InventorySpaceQuadSize { get; } = 35;
    public Point InventoryOffset { get; } = new Point(1730, 816);
    public Point ChaosSaleOffset { get; } = new Point(41, 233);
    public Point EssenceTab { get; } = new Point(420, 192);
    public Point Stash { get; } = new Point(1270, 714);
    public Point Waypoint { get; } = new Point(1178, 563);
    public Point Part1 { get; } = new Point(293, 157);
    public Point Act1 { get; } = new Point(239, 205);
    public Point Act1Town { get; } = new Point(276, 594);
    public Point TradeAccept { get; } = new Point(505, 1112);
    public Point TradeRequestAccept { get; } = new Point(2106, 1046);
    public Point CurrencyTab { get; } = new Point(87, 193);
    public Point ChaosSaleTab { get; } = new Point(315, 193);
    public Point ItemSaleTab { get; } = new Point(175, 193);
    public Point PartyPortrait { get; } = new Point(64, 292);
    public Point LeaveParty { get; } = new Point(228, 834);
    public Point SetPriceMenuOffset { get; } = new Point(-127, 115);
    public Point SetPriceMenuValueOffset { get; } = new Point(-187, 224);
    public Point SetPriceMenuClearOffset { get; } = new Point(-187, 155);
    public int SetPriceValueOffset { get; } = 225;
    public Point SetPriceTypeOffset { get; } = new Point(117, 118);
    public Point SetPriceTypeValueOffset { get; } = new Point(215, 253);
    public Point SetPriceTypeValueBottomOffset { get; } = new Point(215, -239);
    public Point TheirTradeOffset { get; } = new Point(447, 305);
    public Point PartyTab { get; } = new Point(445, 242);
    public int ThresholdXOffset { get; set; } = 250;
    public int NewXOffset { get; set; } = 232;
    public int ThresholdYOffset { get; set; } = 800;
}
