using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class InvitesPanelWrapper : ElementWrapper
{
    public InviteWrapper[] Invites => Children.Where(child => Convert.ToInt32(child.Height) == 235 && child.ChildCount == 3)
        .Select(child => child.AsObject<InviteWrapper>()).ToArray();

    public InviteWrapper[] GetPlayerInvites(string inviteTitle) => Invites.Where(invite => invite.InviteTitle.Equals(inviteTitle)).ToArray();
}
