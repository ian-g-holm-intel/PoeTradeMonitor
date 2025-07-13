using PoeHudWrapper.MemoryObjects;
using System.Drawing;

namespace PoeHudWrapper.Elements;

public class PartyElementWrapper : ElementWrapper
{
    public PartyMemberWrapper[] PartyMembers => GetChildFromIndices(0, 0)?.Children?.Select(child => child.AsObject<PartyMemberWrapper>())?.ToArray() ?? [];
    public string[] PartyMemberNames => PartyMembers.Where(member => !member.LeftParty).Select(member => member.Name).ToArray();
    public string PartyMemberZone(string characterName) => PartyMembers.SingleOrDefault(member => member.Name.Equals(characterName, System.StringComparison.OrdinalIgnoreCase))?.Location ?? "";
    public Point PartyMemberPortalButtonLocation(string characterName) => PartyMembers.SingleOrDefault(member => member.Name.Equals(characterName, System.StringComparison.OrdinalIgnoreCase))?.PortalButtonLocation ?? new Point(-1, -1);
}
