using ExileCore.Shared.Helpers;
using GameOffsets;
using PoeHudWrapper.MemoryObjects;
using System.Numerics;

namespace PoeHudWrapper.Elements;

public class SubMapWrapper : ElementWrapper
{
    public const double CameraAngle = 38.7 * Math.PI / 180;
    public static readonly float CameraAngleCos = (float)Math.Cos(CameraAngle);
    public static readonly float CameraAngleSin = (float)Math.Sin(CameraAngle);

    public Vector2 ShiftNum => M.Read<Vector2>(Address + MapSubElement.MapShift);
    public Vector2 DefaultShiftNum => M.Read<Vector2>(Address + MapSubElement.DefaultMapShift);
    public float Zoom => M.Read<float>(Address + MapSubElement.MapZoom);
    public float MapScale => GameWrapper.TheGame.IngameState.Camera.Height / 677f * Zoom;
    public Vector2 MapCenter => GetClientRect().TopLeft.ToVector2Num() + ShiftNum + DefaultShiftNum;
}
