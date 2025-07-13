using GameOffsets;
using PoeHudWrapper.MemoryObjects;

namespace PoeHudWrapper.Elements;

public class MapWrapper : ElementWrapper
{
    private SubMapWrapper _largeMap;
    private SubMapWrapper _smallMap;
    public SubMapWrapper LargeMap => _largeMap ??= ReadObjectAt<SubMapWrapper>(MapElement.LargeMapOffset);
    public float LargeMapShiftX => M.Read<float>(LargeMap.Address + MapSubElement.MapShiftX);
    public float LargeMapShiftY => M.Read<float>(LargeMap.Address + MapSubElement.MapShiftY);
    public float LargeMapZoom => M.Read<float>(LargeMap.Address + MapSubElement.MapZoom);
    public SubMapWrapper SmallMiniMap => _smallMap ??= ReadObjectAt<SubMapWrapper>(MapElement.SmallMapOffset);
    public float SmallMinMapX => M.Read<float>(SmallMiniMap.Address + MapSubElement.MapShiftX);
    public float SmallMinMapY => M.Read<float>(SmallMiniMap.Address + MapSubElement.MapShiftY);
    public float SmallMinMapZoom => M.Read<float>(SmallMiniMap.Address + MapSubElement.MapZoom);
    public ElementWrapper OrangeWords => ReadObjectAt<ElementWrapper>(MapElement.OrangeWordsOffset);
    public ElementWrapper BlueWords => ReadObjectAt<ElementWrapper>(MapElement.BlueWordsOffset);
}