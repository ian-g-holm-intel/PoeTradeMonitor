using System.Collections;
using System.Numerics;
using System.Runtime.InteropServices;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;

namespace PoeHudWrapper.MemoryObjects;

public class ServerPlayerDataWrapper : StructuredRemoteMemoryObject<ServerPlayerDataOffsets>
{
    public CharacterClass Class => (CharacterClass)(Structure.PlayerClass & 0x0F);
    public int Level => Structure.CharacterLevel;
    public int PassiveRefundPointsLeft => Structure.PassiveRefundPointsLeft;
    public int QuestPassiveSkillPoints => Structure.QuestPassiveSkillPoints;
    public int TotalAscendencyPoints => Structure.TotalAscendencyPoints;
    public int SpentAscendencyPoints => Structure.SpentAscendencyPoints;
    public NativePtrArray AllocatedPassivesIds => Structure.PassiveSkillIds;
}

public class ServerDataWrapper : RemoteMemoryObject
{
    private readonly List<Player> _nearestPlayers = new List<Player>();
    public ServerPlayerDataWrapper PlayerInformation => GetObject<ServerPlayerDataWrapper>(ServerDataStruct.PlayerRelatedData);
    public ServerDataOffsets ServerDataStruct => M.Read<ServerDataOffsets>(Address + ServerDataOffsets.Skip);
    public ushort TradeChatChannel => ServerDataStruct.TradeChatChannel;
    public ushort GlobalChatChannel => ServerDataStruct.GlobalChatChannel;
    public byte MonsterLevel => ServerDataStruct.MonsterLevel;
    public int DialogDepth => ServerDataStruct.DialogDepth;

    // moved, needs fixed
    // public CharacterClass PlayerClass => (CharacterClass)(ServerDataStruct.PlayerClass & 0xF);

    //if 51 - more than 50 monsters remaining (no exact number)
    //if 255 - not supported for current map (town or scenary map)
    public byte MonstersRemaining => ServerDataStruct.MonstersRemaining;
    public ushort CurrentSulphiteAmount => ServerDataStruct.CurrentSulphiteAmount;
    public int CurrentAzuriteAmount => ServerDataStruct.CurrentAzuriteAmount;

    public int CurrentWildWisps => ServerDataStruct.CurrentWildWisps;
    public int CurrentVividWisps => ServerDataStruct.CurrentVividWisps;
    public int CurrentPrimalWisps => ServerDataStruct.CurrentPrimalWisps;

    [Obsolete]
    public SharpDX.Vector2 WorldMousePosition => ServerDataStruct.WorldMousePosition.ToSharpDx();

    public Vector2 WorldMousePositionNum => ServerDataStruct.WorldMousePosition;
    public byte EaterOfWorldsCounter => ServerDataStruct.EaterOfWorldsCounter;
    public byte SearingExarchCounter => ServerDataStruct.SearingExarchCounter;

    public IList<Player> NearestPlayers
    {
        get
        {
            if (Address == 0) return new List<Player>();
            var startPtr = ServerDataStruct.NearestPlayers.First;
            var endPtr = ServerDataStruct.NearestPlayers.Last;
            startPtr += 16; //Don't ask me why. Just skipping first 2

            //Sometimes wrong offsets and read 10000000+ objects
            if (startPtr < Address || (endPtr - startPtr) / 16 > 50)
                return _nearestPlayers;

            _nearestPlayers.Clear();

            for (var addr = startPtr; addr < endPtr; addr += 16) //16 because we are reading each second pointer (pointer vectors)
            {
                _nearestPlayers.Add(ReadObject<Player>(addr));
            }

            return _nearestPlayers;
        }
    }

    public List<EntityEffect> EntityEffects => M.ReadStructsArray<EntityEffect>(ServerDataStruct.EntityEffects.First, ServerDataStruct.EntityEffects.Last, 0xA8, null);
    public List<PlacedCurrencyExchangeOrder> PlacedCurrencyExchangeOrders => M.ReadRMOStdVector<PlacedCurrencyExchangeOrder>(ServerDataStruct.PlacedCurrencyOrders, 0x58).ToList();

    public int GetBeastCapturedAmount(BestiaryCapturableMonster monster)
    {
        return M.Read<int>(Address + 0x5240 + monster.Id * 4);
    }

    public Dictionary<QuestFlag, bool> QuestFlags => GetQuestFlags();

    private Dictionary<QuestFlag, bool> GetQuestFlags()
    {
        var vector = M.Read<StdVector>(ServerDataStruct.QuestFlagsPtr + ServerDataOffsets.QuestFlagsOffset);
        var store = M.ReadStdVector<QuestFlagStore>(vector);
        var flags = store.SelectMany(s => Enumerable.Range(0, 64).Select(i => ((QuestFlag)(s.PartialId * 64 + i), ((s.Flags >> i) & 1) == 1)))
            .ToDictionary(x => x.Item1, x => x.Item2);
        return flags;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 9)]
    private struct QuestFlagStore
    {
        [FieldOffset(0x0)] public byte PartialId;
        [FieldOffset(0x1)] public ulong Flags;
    }

    #region PlayerData

    public ushort LastActionId => ServerDataStruct.LastActionId;
    public int Gold => ServerDataStruct.Gold;
    public int CharacterLevel => PlayerInformation.Level;
    public int PassiveRefundPointsLeft => PlayerInformation.PassiveRefundPointsLeft;
    public int QuestPassiveSkillPoints => PlayerInformation.QuestPassiveSkillPoints;
    public int TotalAscendencyPoints => PlayerInformation.TotalAscendencyPoints;
    public int SpentAscendencyPoints => PlayerInformation.SpentAscendencyPoints;
    public PartyAllocation PartyAllocationType => (PartyAllocation)ServerDataStruct.PartyAllocationType;
    public string League => ServerDataStruct.League.ToString(M);
    public PartyStatus PartyStatusType => (PartyStatus)this.ServerDataStruct.PartyStatusType;
    public bool IsInGame => NetworkState == NetworkStateE.Connected;
    public NetworkStateE NetworkState => (NetworkStateE)this.ServerDataStruct.NetworkState;
    public int Latency => ServerDataStruct.Latency;
    public string Guild => NativeStringReader.ReadString(ServerDataStruct.GuildName, M);
    public string GuildTag => NativeStringReader.ReadString(ServerDataStruct.GuildName + 0x20, M);
    public BitArray WaypointsUnlockState => new BitArray(M.ReadBytes(Address + ServerDataOffsets.WaypointsUnlockStateOffset, 24));
    public int InstanceId => ServerDataStruct.InstanceId;
    public IList<ushort> SkillBarIds
    {
        get
        {
            if (Address == 0) return new List<ushort>();

            var readAddr = ServerDataStruct.SkillBarIds;

            var res = new List<ushort>
            {
                readAddr.SkillBar1,
                readAddr.SkillBar2,
                readAddr.SkillBar3,
                readAddr.SkillBar4,
                readAddr.SkillBar5,
                readAddr.SkillBar6,
                readAddr.SkillBar7,
                readAddr.SkillBar8,
                readAddr.SkillBar9,
                readAddr.SkillBar10,
                readAddr.SkillBar11,
                readAddr.SkillBar12,
                readAddr.SkillBar13
            };

            return res;
        }
    }

    public IList<ushort> PassiveSkillIds
    {
        get
        {
            if (Address == 0)
                return null;
            var totalStats = PlayerInformation.AllocatedPassivesIds.ElementCount<ushort>();
            if (totalStats < 0 || totalStats > 500)
                return new List<ushort>();
            return M.ReadStdVector<ushort>(PlayerInformation.AllocatedPassivesIds);
        }
    }

    public IList<ushort> AtlasPassiveSkillIds
    {
        get
        {
            if (Address == 0)
                return null;
            var atlasPassivePtr = M.Read<NativePtrArray>(ServerDataStruct.AtlasTreeContainerPtrs[ServerDataStruct.ActiveAtlasTreeContainerIndex].TreePtr + ServerDataOffsets.AtlasPassivesListOffset);
            var totalStats = atlasPassivePtr.ElementCount<ushort>();
            if (totalStats < 0 || totalStats > 500)
                return new List<ushort>();
            return M.ReadStdVector<ushort>(atlasPassivePtr);
        }
    }

    #endregion

    #region Stash Tabs

    public IList<ServerStashTabWrapper> PlayerStashTabs => GetStashTabs(ServerDataStruct.PlayerStashTabs);

    public IList<ServerStashTabWrapper> GuildStashTabs => GetStashTabs(ServerDataStruct.GuildStashTabs);

    private List<ServerStashTabWrapper> GetStashTabs(StdVector vector)
    {
        var firstAddr = vector.First;
        var lastAddr = vector.Last;

        if (firstAddr <= 0 || lastAddr <= 0) return null;

        var len = lastAddr - firstAddr;

        if (len <= 0 || len > 0xFFFF)
            return [];

        return M.ReadStructsArray<ServerStashTabWrapper>(firstAddr, lastAddr, ServerStashTabOffsets.StructSize, GameWrapper.TheGame);
    }

    #endregion

    #region Inventories

    private IList<InventoryHolderWrapper> ReadInventoryHolders(NativePtrArray array)
    {
        var firstAddr = array.First;
        var lastAddr = array.Last;
        //Sometimes wrong offsets and read 10000000+ objects
        if (firstAddr == 0 || lastAddr <= firstAddr || (lastAddr - firstAddr) / InventoryHolderWrapper.StructSize > 1024)
            return new List<InventoryHolderWrapper>();

        return M.ReadStructsArray<InventoryHolderWrapper>(firstAddr, lastAddr, InventoryHolderWrapper.StructSize, this).ToList();
    }

    public IList<InventoryHolderWrapper> PlayerInventories => ReadInventoryHolders(ServerDataStruct.PlayerInventories);
    public IList<InventoryHolderWrapper> NPCInventories => ReadInventoryHolders(ServerDataStruct.NPCInventories);
    public IList<InventoryHolderWrapper> GuildInventories => ReadInventoryHolders(ServerDataStruct.GuildInventories);

    public AncestorServerData AncestorFights
    {
        get
        {
            var ptrs = M.ReadStdVector<long>(ServerDataStruct.AncestorFightInfoList);
            if (ptrs.Length == 0) return null;
            //0x8D comes from the network. Fall back to the first element in case it changes but the normal layout stays
            var ptr = ptrs.FirstOrDefault(p => M.Read<int>(p + 0x8) == 0x8D, ptrs[0]);
            return GetObject<AncestorServerData>(ptr);
        }
    }

    #region Utils functions

    public ServerInventoryWrapper GetPlayerInventoryBySlot(InventorySlotE slot)
    {
        foreach (var inventory in PlayerInventories)
        {
            if (inventory.Inventory.InventSlot == slot)
                return inventory.Inventory;
        }

        return null;
    }

    public ServerInventoryWrapper GetPlayerInventoryByType(InventoryTypeE type)
    {
        foreach (var inventory in PlayerInventories)
        {
            if (inventory.Inventory.InventType == type)
                return inventory.Inventory;
        }

        return null;
    }

    public ServerInventoryWrapper GetPlayerInventoryBySlotAndType(InventoryTypeE type, InventorySlotE slot)
    {
        foreach (var inventory in PlayerInventories)
        {
            if (inventory.Inventory.InventType == type && inventory.Inventory.InventSlot == slot)
                return inventory.Inventory;
        }

        return null;
    }

    #endregion

    #endregion

    #region Completed Areas

    [Obsolete]
    public IList<WorldAreaWrapper> ShapedMaps => new List<WorldAreaWrapper>(); // GetAreas(ServerDataStruct.ShapedAreas);

    [Obsolete]
    public IList<WorldAreaWrapper> ElderGuardiansAreas => new List<WorldAreaWrapper>(); // GetAreas(ServerDataStruct.ElderGuardiansAreas);

    [Obsolete]
    public IList<WorldAreaWrapper> MasterAreas => new List<WorldAreaWrapper>(); // GetAreas(ServerDataStruct.MasterAreas);

    [Obsolete]
    public IList<WorldAreaWrapper> ShaperElderAreas => new List<WorldAreaWrapper>(); // GetAreas(ServerDataStruct.ElderInfluencedAreas);

    public IList<WorldAreaWrapper> GetAreas(long address)
    {
        if (Address == 0 || address == 0)
            return new List<WorldAreaWrapper>();

        var res = new List<WorldAreaWrapper>();
        var listStart = M.Read<long>(address);
        var error = 0;

        if (listStart == 0)
            return res;

        var addr = listStart;

        do
        {
            if (addr == 0)
                return res;

            var areaAddr = M.Read<long>(addr + 0x10);

            if (areaAddr == 0)
                break;

            var byAddress = GameWrapper.TheGame.Files.WorldAreas.GetByAddress(areaAddr);

            if (byAddress != null)
                res.Add(byAddress);

            error++;

            //Sometimes wrong offsets and read 10000000+ objects
            if (error > 1024)
            {
                res = new List<WorldAreaWrapper>();

                break;
            }

            addr = M.Read<long>(addr);
        } while (addr != listStart);

        return res;
    }

    #endregion
}