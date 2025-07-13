using ExileCore.PoEMemory;
using GameOffsets;
using ExileCore.Shared.Helpers;
using PoeHudWrapper.Elements;

namespace PoeHudWrapper.MemoryObjects;

public class IngameStateWrapper : RemoteMemoryObject
{
    private static readonly int WorldDataOffset = Extensions.GetOffset<IngameStateOffsets>(x => x.WorldData);
    private static readonly int CameraOffset = Extensions.GetOffset<WorldDataOffsets>(x => x.Camera);

    private IngameStateOffsets IngameStateOffsets => M.Read<IngameStateOffsets>(Address /*+M.offsets.IgsOffsetDelta*/);
    public CameraWrapper Camera => GetObject<CameraWrapper>(M.Read<long>(Address + WorldDataOffset) + CameraOffset);
    public IngameDataWrapper Data => GetObject<IngameDataWrapper>(IngameStateOffsets.Data);
    public bool InGame => ServerData.IsInGame;
    public ServerDataWrapper ServerData => Data.ServerData;
    public IngameUIElementsWrapper IngameUi => GetObject<IngameUIElementsWrapper>(IngameStateOffsets.IngameUi);
    public ElementWrapper UIRoot => GetObject<ElementWrapper>(IngameStateOffsets.UIRoot);

    /// <summary>
    /// Current hover UI ElementWrapper (stash, flasks, inventory etc)
    /// </summary>
    public ElementWrapper UIHover => GetObject<ElementWrapper>(IngameStateOffsets.UIHover);
    public float UIHoverX => IngameStateOffsets.UIHoverPos.X;
    public float UIHoverY => IngameStateOffsets.UIHoverPos.Y;
    /// <summary>
    /// Current hover ElementWrapper (broader than UIHover. Also includes labels)
    /// </summary>
    public ElementWrapper UIHoverElementWrapper => GetObject<ElementWrapper>(IngameStateOffsets.UIHoverElement);
    public ElementWrapper UIHoverTooltip => UIHoverElementWrapper;
    public float CurentUElementWrapperPosX => IngameStateOffsets.CurentUIElementPos.X;
    public float CurentUElementWrapperPosY => IngameStateOffsets.CurentUIElementPos.Y;
    public int MousePosX => IngameStateOffsets.MouseGlobal.X;
    public int MousePosY => IngameStateOffsets.MouseGlobal.Y;
    public long EntityLabelMap => M.Read<EntityLabelMapOffsets>(IngameStateOffsets.EntityLabelMap).EntityLabelMap;
    public TimeSpan TimeInGame => TimeSpan.FromSeconds(IngameStateOffsets.TimeInGameF);
    public float TimeInGameF => IngameStateOffsets.TimeInGameF;
}
