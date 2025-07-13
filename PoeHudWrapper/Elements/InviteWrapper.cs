using PoeHudWrapper.MemoryObjects;
using System.Drawing;

namespace PoeHudWrapper.Elements;

public class InviteWrapper : ElementWrapper
{
    public string InviteTitle => GetChildFromIndices(0, 1)?.Text ?? "";

    private string accountName;
    public string AccountName => string.IsNullOrEmpty(accountName) ? (accountName = GetChildFromIndices(0, 0, 1)?.Text ?? "") : accountName;

    private string characterName;
    public string CharacterName => string.IsNullOrEmpty(characterName) ? (characterName = GetChildFromIndices(1, 0, 0, 0)?.Text ?? "") : characterName;
    public Point AcceptButtonLocation => GetChildFromIndices(2, 0)?.Center ?? new Point(-1, -1);
    public Point DeclineButtonLocation => GetChildFromIndices(2, 1)?.Center ?? new Point(-1, -1);
}
