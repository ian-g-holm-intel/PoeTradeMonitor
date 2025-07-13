using ExileCore;
using ExileCore.PoEMemory;
using System.Diagnostics;

namespace PoeHudWrapper;

public interface IMemoryProvider
{
    Memory GetMemory(CoreSettings settings);
}

public class MemoryProvider : IMemoryProvider
{
    private readonly object syncLock = new object();
    private Memory memory;

    public Memory GetMemory(CoreSettings settings)
    {
        lock (syncLock)
        {
            if (memory == null)
            {
                memory = FindPoe(settings);
                if (memory == null)
                    throw new InvalidOperationException("Failed to initialize PoeHud");
            }
            return memory;
        }
    }

    private Memory FindPoe(CoreSettings settings)
    {
        var process = Process.GetProcessesByName(Offsets.Regular.ExeName).FirstOrDefault();
        if (process != null && process.Id != 0)
            return new Memory(process, Offsets.Regular, settings);
        return null;
    }
}
