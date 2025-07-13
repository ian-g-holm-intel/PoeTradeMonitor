using PoeHudWrapper;
using System;
using System.Collections.Generic;
using Serilog;
using PoeHudWrapper.MemoryObjects;
using Microsoft.Extensions.DependencyInjection;
using ExileCore.PoEMemory;
using ExileCore.Shared.Enums;
using System.Linq;
using PoeHudWrapper.Elements.InventoryElements;
using PoeHudWrapper.Elements;
using ExileCore.Shared.Helpers;
using GameOffsets.Native;

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
            new Bootstrapper().RegisterServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var gameWrapper = serviceProvider.GetRequiredService<IGameWrapper>();
            gameWrapper.Initialize();
            var poeHudWrapper = serviceProvider.GetRequiredService<IPoeHudWrapper>();
            var ingameUI = gameWrapper.IngameState.IngameUi;

            var index = ingameUI.GetRemoteMemoryObjectAddress(0x1560E429D20);

            for (int i = 0x2E8; i < 0xFFF; i++)
            {
                var chatInputElement = ingameUI.ChatPanel.ReadObjectAt<Element>(i);
                var text = chatInputElement.Text;
                if (text == "test")
                {

                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Console.ReadLine();
        }
    }

    private static NormalInventoryItemWrapper GetInventoryItem(IGameWrapper gameWrapper)
    {
        var inventorySlotItems = gameWrapper.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].ServerInventory.InventorySlotItems;
        var visibleInventoryItems = gameWrapper.IngameState.IngameUi.InventoryPanel[InventoryIndex.PlayerInventory].VisibleInventoryItems;
        var itemEntity = inventorySlotItems.SingleOrDefault(i => i.Location.InventoryPosition.X == 0 && i.Location.InventoryPosition.Y == 0)?.Item;
        var item = visibleInventoryItems?.SingleOrDefault(i => i?.Entity?.Address != null && itemEntity != null && i.Entity.Address == itemEntity.Address);

        return item;
    }

    private static List<int> FindPartyLeaderName(ServerDataWrapper serverData)
    {
        for (int i = 0; i < 0xFFFFF; i++)
        {
            var name = serverData.M.Read<NativeUtf16Text>(serverData.Address + i).ToString(serverData.M);
            if (name == "FishTester")
            {
                return new List<int>() { i };
            }
        }
        return new List<int>();
    }

    private static List<int> FindCharacterLevel(ServerDataWrapper serverData)
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

    private static long GetPartyMemberPortraitAddress(PartyElementWrapper partyElement)
    {
        var firstPartyMember = partyElement.PartyMembers.First();
        var portraitElement = firstPartyMember.GetChildFromIndices(1, 0);
        return portraitElement.Address;
    }

    public static List<int> FindPartyMemberSaturated(PartyElementWrapper partyElement, int length, int numberOfLoops)
    {
        // Store first read values for all loops to check consistency
        var firstReadValues = new byte[numberOfLoops][];
        // Store second read values for all loops to check consistency
        var secondReadValues = new byte[numberOfLoops][];

        // Perform all reads first
        for (int loop = 0; loop < numberOfLoops; loop++)
        {
            firstReadValues[loop] = partyElement.M.ReadBytes(GetPartyMemberPortraitAddress(partyElement), length);
            secondReadValues[loop] = partyElement.M.ReadBytes(GetPartyMemberPortraitAddress(partyElement), length);
        }

        var validOffsets = new List<int>();

        // Check each byte position
        for (int byteIndex = 0; byteIndex < length; byteIndex++)
        {
            bool isValidOffset = true;

            // Get the reference values from the first loop
            byte firstReadReference = firstReadValues[0][byteIndex];
            byte secondReadReference = secondReadValues[0][byteIndex];

            // Must be different between first and second read
            if (firstReadReference == secondReadReference)
            {
                continue;
            }

            // Check all loops for this byte position
            for (int loop = 1; loop < numberOfLoops; loop++)
            {
                // Check if first read matches reference
                if (firstReadValues[loop][byteIndex] != firstReadReference)
                {
                    isValidOffset = false;
                    break;
                }

                // Check if second read matches reference
                if (secondReadValues[loop][byteIndex] != secondReadReference)
                {
                    isValidOffset = false;
                    break;
                }
            }

            if (isValidOffset)
            {
                var firstValue = firstReadValues[0][byteIndex];
                var secondValue = secondReadValues[0][byteIndex];
                validOffsets.Add(byteIndex);
            }
        }

        return validOffsets;
    }

    public static List<int> FindDynamicOffsets(IGameWrapper gameWrapper, int length, int numberOfLoops)
    {
        // Store first read values for all loops to check consistency
        var firstReadValues = new byte[numberOfLoops][];
        // Store second read values for all loops to check consistency
        var secondReadValues = new byte[numberOfLoops][];

        // Perform all reads first
        for (int loop = 0; loop < numberOfLoops; loop++)
        {
            var firstItem = GetInventoryItem(gameWrapper);
            if (firstItem == null)
                return new List<int>();
            firstReadValues[loop] = firstItem.M.ReadBytes(firstItem.Address, length);

            var secondItem = GetInventoryItem(gameWrapper);
            if (secondItem == null)
                return new List<int>();
            secondReadValues[loop] = secondItem.M.ReadBytes(secondItem.Address, length);
        }

        var validOffsets = new List<int>();

        // Check each byte position
        for (int byteIndex = 0; byteIndex < length; byteIndex++)
        {
            bool isValidOffset = true;

            // Get the reference values from the first loop
            byte firstReadReference = firstReadValues[0][byteIndex];
            byte secondReadReference = secondReadValues[0][byteIndex];

            // Must be different between first and second read
            if (firstReadReference == secondReadReference)
            {
                continue;
            }

            // Check all loops for this byte position
            for (int loop = 1; loop < numberOfLoops; loop++)
            {
                // Check if first read matches reference
                if (firstReadValues[loop][byteIndex] != firstReadReference)
                {
                    isValidOffset = false;
                    break;
                }

                // Check if second read matches reference
                if (secondReadValues[loop][byteIndex] != secondReadReference)
                {
                    isValidOffset = false;
                    break;
                }
            }

            if (isValidOffset)
            {
                var firstValue = firstReadValues[0][byteIndex];
                var secondValue = secondReadValues[0][byteIndex];
                validOffsets.Add(byteIndex);
            }
        }

        return validOffsets;
    }

    private static List<int> FindTogglingBits(NormalInventoryItemWrapper item, int startingOffset, int length, int numberOfLoops)
    {
        // Store initial state
        byte[] previousState = item.M.ReadBytes(item.Address + startingOffset, length);

        // Track which bytes have their IsSelected bit (bit 5) toggling consistently
        Dictionary<int, int> toggleCount = new Dictionary<int, int>();

        // Run the specified number of loops
        for (int loop = 0; loop < numberOfLoops; loop++)
        {
            // Read current state
            byte[] currentState = item.M.ReadBytes(item.Address + startingOffset, length);

            // Compare each byte
            for (int byteIndex = 0; byteIndex < length; byteIndex++)
            {
                // Check if bit 5 (IsSelected) has changed
                bool previousSelected = (previousState[byteIndex] >> 5 & 1) == 0;
                bool currentSelected = (currentState[byteIndex] >> 5 & 1) == 0;

                if (previousSelected != currentSelected)
                {
                    // Increment toggle count for this byte
                    if (!toggleCount.ContainsKey(byteIndex))
                    {
                        toggleCount[byteIndex] = 0;
                    }
                    toggleCount[byteIndex]++;
                }
            }

            // Current state becomes previous state for next iteration
            previousState = currentState;
        }

        // Find bytes where IsSelected toggled every loop (numberOfLoops - 1 toggles)
        List<int> consistentlyTogglingBytes = new List<int>();
        foreach (var kvp in toggleCount)
        {
            if (kvp.Value == numberOfLoops - 1)  // We expect (loops - 1) toggles for consistent toggling
            {
                consistentlyTogglingBytes.Add(kvp.Key);
            }
        }

        return consistentlyTogglingBytes;
    }
}

public static class ExtensionMethods
{
    public static Stack<int> SearchChildIndicies(this ElementWrapper element, long targetAddress)
    {
        var stack = new Stack<int>();

        SearchChildForIndex(element, targetAddress, stack);

        return stack;
    }

    private static bool SearchChildForIndex(ElementWrapper element, long targetAddress, Stack<int> stack)
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

    public static List<long> GetRemoteMemoryObjectAddress(this RemoteMemoryObject remoteMemoryObject, long targetAddress)
    {
        var offsets = new List<long>();

        remoteMemoryObject.GetRemoteMemoryObjectAddress(targetAddress, offsets);

        return offsets;
    }

    private static List<long> GetRemoteMemoryObjectAddress(this RemoteMemoryObject remoteMemoryObject, long targetAddress, List<long> offsets)
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
