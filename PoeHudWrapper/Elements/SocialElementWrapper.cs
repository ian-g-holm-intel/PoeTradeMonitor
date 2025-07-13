using ExileCore.Shared.Enums;
using PoeHudWrapper.MemoryObjects;
using System.Drawing;

namespace PoeHudWrapper.Elements;

public class SocialElementWrapper : ElementWrapper
{
    public Point FriendsTabLocation => SocialTabs?.FirstOrDefault(tab => tab?.Text == "Friends")?.Center ?? new Point(-1, -1);
    public Point GuildTabLocation => SocialTabs?.FirstOrDefault(tab => tab?.Text == "Guild")?.Center ?? new Point(-1, -1);
    public Point PartyTabLocation => SocialTabs?.FirstOrDefault(tab => tab?.Text == "Current Party" || tab?.Text == "Create Party")?.Center ?? new Point(-1, -1);
    public Point PublicPartiesTabLocation => SocialTabs?.FirstOrDefault(tab => tab?.Text == "Public Parties")?.Center ?? new Point(-1, -1);
    public IEnumerable<ElementWrapper> SocialTabs => GetChildFromIndices(2, 4, 1, 0)?.Children?.Select(tab => tab.GetChildFromIndices(0, 1));
    public bool PartyTabVisible => PartyTab?.IsVisible ?? false;
    public ElementWrapper PartyTab => GetChildFromIndices(2, 4, 1, 1, 2, 0, 1, 0, 1);
    public SocialPartyMemberWrapper PartyLeader => PartyTab?.GetChildFromIndices(0, 1, 0)?.Children?.FirstOrDefault()?.AsObject<SocialPartyMemberWrapper>();
    public SocialPartyMemberWrapper[] PartyMembers => PartyTab?.GetChildFromIndices(1, 1, 0)?.Children?.Select(child => child.AsObject<SocialPartyMemberWrapper>())?.ToArray() ?? new SocialPartyMemberWrapper[0];

    public PartyStatus? PartyStatus
    {
        get
        {
            if (PartyTab?.ChildCount < 4)
                return null;

            Point partyLeaderLeaveButtonLocation = PartyLeader?.ButtonLocation ?? new Point(-1, -1);
            if (partyLeaderLeaveButtonLocation != new Point(-1, -1))
                return ExileCore.Shared.Enums.PartyStatus.PartyLeader;

            Point partyMemberLeaveButtonLocation = PartyMembers.FirstOrDefault(member => member.ButtonText.Equals("leave", StringComparison.OrdinalIgnoreCase))?.ButtonLocation ?? new Point(-1, -1);
            if (partyMemberLeaveButtonLocation != new Point(-1, -1))
                return ExileCore.Shared.Enums.PartyStatus.PartyMember;

            if (PendingInviteElements.Length > 0)
                return ExileCore.Shared.Enums.PartyStatus.Invited;

            return ExileCore.Shared.Enums.PartyStatus.None;
        }
    }

    public Point LeavePartyButtonLocation
    {
        get
        {
            Point partyLeaderLeaveButtonLocation = PartyLeader?.ButtonLocation ?? new Point(-1, -1);
            if (partyLeaderLeaveButtonLocation != new Point(-1, -1))
            {
                return partyLeaderLeaveButtonLocation!;
            }

            Point partyMemberLeaveButtonLocation = PartyMembers.FirstOrDefault(member => member.ButtonText.Equals("leave", StringComparison.OrdinalIgnoreCase))?.ButtonLocation ?? new Point(-1, -1);
            if (partyMemberLeaveButtonLocation != new Point(-1, -1))
            {
                return partyMemberLeaveButtonLocation!;
            }
            else
            {
                return new Point(-1, -1);
            }
        }
    }

    private ElementWrapper[] PendingInviteElements => PartyTab?.ChildCount >= 4 ? PartyTab.GetChildFromIndices(3, 1, 0).Children.ToArray() : new ElementWrapper[0];
    public string[] PendingInvites => PendingInviteElements.Where(element => element.ChildCount == 1).Select(invite => invite.GetChildFromIndices(0, 1, 0, 0, 0).Text).ToArray();
    public Point GetPartyInviteAcceptLocation(string characterName)
    {
        var pendingElements = PendingInviteElements;
        for (int i = 0; i < pendingElements.Length; i++)
        {
            var element = pendingElements[i];
            if (element.ChildCount == 1)
            {
                var name = element.GetChildFromIndices(0, 1, 0, 0, 0).Text;
                if (name.Equals(characterName, StringComparison.OrdinalIgnoreCase))
                {
                    return element.GetChildFromIndices(0, 2, 0).Center;
                }
            }
        }
        return new Point(-1, -1);
    }

    public Point GetPartyInviteDeclineLocation(string characterName)
    {
        var pendingElements = PendingInviteElements;
        for (int i = 0; i < pendingElements.Length; i++)
        {
            var element = pendingElements[i];
            if (element.ChildCount == 1)
            {
                var name = element.GetChildFromIndices(0, 0, 0).Text;
                if (name.Equals(characterName, StringComparison.OrdinalIgnoreCase))
                {
                    return pendingElements[i + 1].GetChildAtIndex(1).Center;
                }
            }
        }
        return new Point(-1, -1);
    }
}
