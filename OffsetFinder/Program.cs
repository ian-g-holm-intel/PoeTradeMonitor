using ExileCore2;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.Elements;
using ExileCore2.PoEMemory.Elements.InventoryElements;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using GameOffsets2.Native;
using Microsoft.Extensions.DependencyInjection;
using PoeHudWrapper;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PoeHUD.OffsetFinder;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddPoeHudWrapper();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var poeHud = serviceProvider.GetRequiredService<IPoeHudWrapper>();
            var serverData = Core.Current.GameController.IngameState.ServerData;
            var offset = FindPartyStatusType(serverData);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.ReadLine();
        }
    }

    private static List<long> FindPartyInformationOffsets(PartyElement partyElement)
    {
        var offsets = new List<long>();
        for (int i = 0x100; i < 0x400; i++)
        {
            var informationDictionary = partyElement.M.ReadRMOStdVector<PartyElementPlayerInfoWrapper>(partyElement.M.Read<StdVector>(partyElement.Address + i), 0x30)
                                                       .DistinctBy(x => x.PlayerName)
                                                       .ToDictionary(x => x.PlayerName, x => x.Info);
            if (informationDictionary.ContainsKey("Garrochu"))
                offsets.Add(i);
        }
        return offsets;
    }

    private static NormalInventoryItem GetInventoryItem()
    {
        var inventoryPanel = Core.Current.GameController.IngameState.IngameUi.InventoryPanel;
        var inventorySlotItems = inventoryPanel[InventoryIndex.PlayerInventory].ServerInventory.InventorySlotItems;
        var visibleInventoryItems = inventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems;
        var itemEntity = inventorySlotItems.SingleOrDefault(i => i.Location.InventoryPosition.X == 0 && i.Location.InventoryPosition.Y == 0)?.Item;
        var item = visibleInventoryItems?.SingleOrDefault(i => i?.Entity?.Address != null && itemEntity != null && i.Entity.Address == itemEntity.Address);

        return item;
    }

    private static List<int> FindPartyLeaderName(ServerData serverData)
    {
        for (int i = 0; i < 0x3000; i++)
        {
            var name = serverData.M.Read<NativeUtf16Text>(serverData.Address + i).ToString(serverData.M);
            if (name == "RangerGOD____")
            {
                return new List<int>() { i };
            }
        }
        return new List<int>();
    }

    private static List<int> FindPartyAllocationType(ServerData serverData)
    {
        var party1Offsets = new List<int>();
        for (int i = 0x1000; i < 0x3000; i++)
        {
            var partyAllocation = serverData.M.Read<PartyAllocation>(serverData.Address + i);
            if (partyAllocation == PartyAllocation.ShortAllocation)
            {
                party1Offsets.Add(i);
            }
        }

        var party2Offsets = new List<int>();
        for (int i = 0x1000; i < 0x3000; i++)
        {
            var partyAllocation = serverData.M.Read<PartyAllocation>(serverData.Address + i);
            if (partyAllocation == PartyAllocation.FreeForAll)
            {
                party2Offsets.Add(i);
            }
        }
        return party1Offsets.Intersect(party2Offsets).ToList();
    }

    private static List<int> FindPartyStatusType(ServerData serverData)
    {
        var party1Offsets = new List<int>();
        for (int i = 0x1000; i < 0x3000; i++)
        {
            var partyStatus = serverData.M.Read<PartyStatus>(serverData.Address + i);
            if (partyStatus == PartyStatus.None)
            {
                party1Offsets.Add(i);
            }
        }

        var party2Offsets = new List<int>();
        for (int i = 0x1000; i < 0x3000; i++)
        {
            var partyStatus = serverData.M.Read<PartyStatus>(serverData.Address + i);
            if (partyStatus == PartyStatus.PartyMember)
            {
                party2Offsets.Add(i);
            }
        }
        return party1Offsets.Intersect(party2Offsets).ToList();
    }

    private static List<int> FindPlayerName(Player player)
    {
        var offsets = new List<int>();
        for (int i = 0; i < 0xFFF; i++)
        {
            var name = player.M.Read<NativeUtf16Text>(player.Address + i).ToString(player.M);
            if (name == "RetikRenameLater")
            {
                offsets.Add(i);
            }
        }
        return offsets;
    }

    private static List<int> FindCharacterLevel(ServerData serverData)
    {
        for (int i = 0; i < 0x2000; i++)
        {
            var level = serverData.M.Read<int>(serverData.Address + i);
            if (level == 95)
            {
                return new List<int>() { i };
            }
        }
        return new List<int>();
    }
}

public static class ExtensionMethods
{
    public static Stack<int> SearchChildIndicies(this Element element, long targetAddress)
    {
        var stack = new Stack<int>();

        SearchChildForIndex(element, targetAddress, stack);

        return stack;
    }

    private static bool SearchChildForIndex(Element element, long targetAddress, Stack<int> stack)
    {
        for (int i = 0; i < element.ChildCount; i++)
        {
            stack.Push(i);
            if (element.Children[i].Address == targetAddress)
                return true;
            else if (element.Children[i].ChildCount > 0 && SearchChildForIndex(element.Children[i], targetAddress, stack))
                return true;
            else
                stack.Pop();
        }
        return false;
    }

    public static List<long> GetRemoteMemoryObjectAddress<T>(this RemoteMemoryObject remoteMemoryObject, long targetAddress) where T : RemoteMemoryObject, new()
    {
        var offsets = new List<long>();

        remoteMemoryObject.GetRemoteMemoryObjectAddress<T>(targetAddress, offsets);

        return offsets;
    }

    private static List<long> GetRemoteMemoryObjectAddress<T>(this RemoteMemoryObject remoteMemoryObject, long targetAddress, List<long> offsets) where T : RemoteMemoryObject, new()
    {
        for (long offset = 0; offset <= 0xFFFFF; offset++)
        {
            var ptr = remoteMemoryObject.M.Read<long>(remoteMemoryObject.Address + offset);
            if (ptr == targetAddress)
            {
                offsets.Add(offset);
            }
        }
        return offsets;
    }

    public static long GetRemoteMemoryObjectNestedAddress<T>(this RemoteMemoryObject remoteMemoryObject, int baseOffset, long targetAddress) where T : RemoteMemoryObject, new()
    {
        for (int offset = 0; offset <= 0xFFFFF; offset++)
        {
            var ptr = remoteMemoryObject.M.Read<long>(remoteMemoryObject.Address + baseOffset, offset);
            if (ptr == targetAddress)
            {
                return offset;
            }
        }
        return 0;
    }
}
