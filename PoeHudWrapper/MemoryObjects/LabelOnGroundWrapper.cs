using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;

namespace PoeHudWrapper.MemoryObjects;

public class LabelOnGroundWrapper : RemoteMemoryObject
{
    private readonly Lazy<string> debug;
    private readonly Lazy<long> labelInfo;

    public LabelOnGroundWrapper()
    {
        labelInfo = new Lazy<long>(GetLabelInfo);

        debug = new Lazy<string>(() =>
        {
            return ItemOnGround.HasComponent<WorldItem>()
                ? ItemOnGround.GetComponent<WorldItem>().ItemEntity?.GetComponent<Base>()?.Name
                : ItemOnGround.Path;
        });
    }

    public bool IsVisible => Label?.IsVisible ?? false;

    public Entity ItemOnGround
    {
        get
        {
            var readObjectAt = ReadObjectAt<Entity>(0x18);
            return readObjectAt;
        }
    }

    public ElementWrapper Label
    {
        get
        {
            var readObjectAt = ReadObjectAt<ElementWrapper>(0x10);
            return readObjectAt;
        }
    }

    //it broke again, feel free to fix
    public bool CanPickUp => true ||
                             Label?.Address is not { } labelAddress ||
                             M.Read<long>(labelAddress + 0x678) == 0;

    public TimeSpan TimeLeft
    {
        get
        {
            // Temporary fix for Pickit
            return TimeSpan.Zero;
            /*
            if (CanPickUp) return new TimeSpan();
            if (labelInfo.Value == 0) return MaxTimeForPickUp;
            var futureTime = M.Read<int>(labelInfo.Value + 0x38);
            return TimeSpan.FromMilliseconds(futureTime - Environment.TickCount);
            */
        }
    }

    //Temp solution for pick it
    public TimeSpan MaxTimeForPickUp =>
        TimeSpan.Zero; // !CanPickUp ? TimeSpan.FromMilliseconds(M.Read<int>(labelInfo.Value + 0x34)) : new TimeSpan();

    private long GetLabelInfo()
    {
        return Label != null ? Label.Address != 0 ? M.Read<long>(Label.Address + 0x3B8) : 0 : 0; // ??? - might need fixed
    }

    public override string ToString()
    {
        return debug.Value;
    }
}
