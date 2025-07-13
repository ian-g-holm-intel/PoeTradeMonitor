using ExileCore.PoEMemory.MemoryObjects;
using GameOffsets;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class EntityLabelWrapper : ElementWrapper
{
    public string Text => NativeStringReader.ReadString(Address + EntityLabelMapOffsets.LabelOffset, M);
    public string Text2 => NativeStringReader.ReadString(Address + EntityLabelMapOffsets.LabelOffset, M);
}
