using PoeHudWrapper.MemoryObjects;
using System.Drawing;

namespace PoeHudWrapper.Elements;

public class SocialPartyMemberWrapper : ElementWrapper
{
    private ElementWrapper CharacterInfo => GetChildFromIndices(0, 1, 0, 0);
    public string Name => CharacterInfo.GetChildAtIndex(0)?.Text ?? "";
    public string Location => CharacterInfo.GetChildFromIndices(2)?.Text ?? "";
    public string ButtonText => GetChildFromIndices(0, 1, 1, 0)?.Text ?? "";
    public Point ButtonLocation => GetChildFromIndices(0, 1, 1)?.Center ?? new Point(-1, -1);
}
