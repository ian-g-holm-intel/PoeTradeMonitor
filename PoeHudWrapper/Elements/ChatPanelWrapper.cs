using GameOffsets;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;
public class ChatPanelWrapper : ElementWrapper
{
    public ElementWrapper ChatTitlePanel => ReadObjectAt<ElementWrapper>(IngameUIElementsOffsets.CHAT_TITLE_OFFSET); // isVisible when chat opened
    public ElementWrapper ChatInputElement => ReadObjectAt<ElementWrapper>(IngameUIElementsOffsets.CHAT_INPUT_OFFSET);

    public PoeChatElementWrapper ChatBox => ReadObjectAt<ElementWrapper>(IngameUIElementsOffsets.CHAT_BOX_OFFSET_1)
        .ReadObjectAt<PoeChatElementWrapper>(IngameUIElementsOffsets.CHAT_BOX_OFFSET_2);

    public string InputText => ChatInputElement.Text;
}
