using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Helpers;
using GameOffsets;
namespace PoeHudWrapper.MemoryObjects;
public class EntityListWrapper : RemoteMemoryObject
{
    public List<Entity> Entities => CollectEntities();
    public List<Entity> CollectEntities()
    {
        if (Address == 0)
            return new List<Entity>();
        var unexploredAddressQueue = new Queue<long>(256);
        var discoveredAddressList = new List<long>(1000);
        var discoveredAddressSet = new HashSet<long>(256);
        var addr = M.Read<long>(Address + 0x8);
        unexploredAddressQueue.Enqueue(addr);
        const int maxEntities = 10000;
        var remainingIterations = maxEntities;
        while (unexploredAddressQueue.Count > 0 && remainingIterations > 0)
        {
            try
            {
                remainingIterations--;
                var nextAddr = unexploredAddressQueue.Dequeue();
                if (!discoveredAddressSet.Add(nextAddr))
                    continue;
                if (nextAddr != 0)
                {
                    var node = M.Read<EntityListOffsets>(nextAddr);
                    var entityAddress = node.Entity;
                    if (entityAddress > 0x1000 && entityAddress < 0x7F0000000000)
                        discoveredAddressList.Add(entityAddress);
                    unexploredAddressQueue.Enqueue(node.FirstAddr);
                    unexploredAddressQueue.Enqueue(node.SecondAddr);
                }
            }
            catch (Exception)
            {
                continue;
            }
        }
        var entities = new List<Entity>();
        foreach (var addrEntity in discoveredAddressList)
        {
            var entity = ParseEntity(addrEntity);
            if (entity != null)
            {
                entities.Add(entity);
            }
        }
        return entities;
    }
    private Entity ParseEntity(long addrEntity)
    {
        var entityId = M.Read<uint>(addrEntity + Extensions.GetOffset<EntityOffsets>(x => x.Id));
        if (entityId <= 0)
            return null;
        var entity = GetObject<Entity>(addrEntity);
        if (entity.Check(entityId))
        {
            entity.IsValid = true;
            return entity;
        }
        return null;
    }
}