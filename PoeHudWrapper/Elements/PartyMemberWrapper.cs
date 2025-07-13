using PoeHudWrapper.MemoryObjects;
using System.Drawing;

namespace PoeHudWrapper.Elements;

public class PartyMemberWrapper : ElementWrapper
{
    public string Name => GetChildAtIndex(0)?.Text  ?? "";
    public string Location => ChildCount > 3 ? (GetChildAtIndex(2)?.Text ?? "") : "";
    public bool LeftParty => GetChildAtIndex(2)?.IsSelected ?? true;
    public Point PortalButtonLocation => GetChildAtIndex(3).Center;
}
