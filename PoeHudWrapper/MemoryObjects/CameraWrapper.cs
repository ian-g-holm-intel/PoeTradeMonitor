using ExileCore.PoEMemory;
using GameOffsets;
using System.Numerics;
using ExileCore;

namespace PoeHudWrapper.MemoryObjects;

public class CameraWrapper : RemoteMemoryObject
{
    private CameraOffsets CameraOffsets => M.Read<CameraOffsets>(Address);
    private Matrix4x4 Matrix => CameraOffsets.MatrixBytes;
    public int Width => CameraOffsets.Width;
    public int Height => CameraOffsets.Height;
    private float HalfWidth => CameraOffsets.Width * 0.5f;
    private float HalfHeight => CameraOffsets.Height * 0.5f;

    public Vector2 WorldToScreen(Vector3 vec)
    {
        try
        {
            Vector2 result;
            var cord = new Vector4(vec, 1);
            cord = Vector4.Transform(cord, Matrix);
            cord = Vector4.Divide(cord, cord.W);
            result.X = (cord.X + 1.0f) * HalfWidth;
            result.Y = (1.0f - cord.Y) * HalfHeight;

            return result;
        }
        catch (Exception ex)
        {
            Core.Logger?.Error($"Camera WorldToScreen {ex}");
        }

        return Vector2.Zero;
    }
}
