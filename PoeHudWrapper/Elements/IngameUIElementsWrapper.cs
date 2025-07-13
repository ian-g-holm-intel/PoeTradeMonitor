using GameOffsets;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class IngameUIElementsWrapper : ElementWrapper
{
    public IngameUIElementsOffsets IngameUIElementsStruct => M.Read<IngameUIElementsOffsets>(Address);
    public TradeWindowWrapper TradeWindow => GetObject<TradeWindowWrapper>(IngameUIElementsStruct.TradeWindow);
    public SocialElementWrapper SocialPanel => GetObject<SocialElementWrapper>(IngameUIElementsStruct.SocialPanel);
    public ChatPanelWrapper ChatPanel => GetObject<ChatPanelWrapper>(IngameUIElementsStruct.ChatBox);
    public ElementWrapper ChatTitlePanel => ChatPanel.ChatTitlePanel;
    public PoeChatElementWrapper ChatBox => ChatPanel.ChatBox;
    public IList<string> ChatMessages => ChatBox.Messages;
    public ElementWrapper OpenLeftPanel => GetObject<ElementWrapper>(IngameUIElementsStruct.OpenLeftPanel);
    public ElementWrapper OpenRightPanel => GetObject<ElementWrapper>(IngameUIElementsStruct.OpenRightPanel);
    public StashElementWrapper StashElement => GetObject<StashElementWrapper>(IngameUIElementsStruct.StashElement);
    public InventoryElementWrapper InventoryPanel => GetObject<InventoryElementWrapper>(IngameUIElementsStruct.InventoryPanel);
    public MapWrapper Map => GetObject<MapWrapper>(IngameUIElementsStruct.Map);
    public InvitesPanelWrapper InvitesPanel => GetObject<InvitesPanelWrapper>(IngameUIElementsStruct.InvitesPanel);
    public ElementWrapper PopUpWindow => GetObject<ElementWrapper>(IngameUIElementsStruct.PopUpWindow);
    public DestroyConfirmationElementWrapper DestroyConfirmationWindow => GetObject<DestroyConfirmationElementWrapper>(IngameUIElementsStruct.DestroyConfirmationWindow);
    public PartyElementWrapper PartyElement => GetObject<PartyElementWrapper>(IngameUIElementsStruct.PartyElement);
    public ElementWrapper HighlightedElement => Root?.GetChildFromIndices(1, 6, 1, 0);
}