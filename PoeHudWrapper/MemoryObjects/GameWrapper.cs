using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets.Objects;
using Microsoft.Extensions.Logging;
using System.Drawing;

namespace PoeHudWrapper.MemoryObjects;

public class GameWrapper : RemoteMemoryObject, IGameWrapper
{
    private Dictionary<GameStateTypes, long> AllGameStates;
    private bool disposedValue;
    private readonly IMemoryProvider memoryProvider;
    private readonly ILogger<GameWrapper> logger;

    public GameWrapper(IMemoryProvider memoryProvider, ILogger<GameWrapper> logger)
    {
        this.memoryProvider = memoryProvider;
        this.logger = logger;
    }

    public void Initialize()
    {
        CoreSettings = new CoreSettings();
        pM = memoryProvider.GetMemory(CoreSettings);
        pCache = new Cache();
        Files = new FilesContainerWrapper(pM, CoreSettings);
        Address = M.Read<long>(M.BaseOffsets[OffsetsName.GameStateOffset] + M.AddressOfProcess);

        AllGameStates = ReadStates(Address);

        TheGame = this;
        logger.LogInformation("GameWrapper Initialized");
    }

    public Rectangle ClientBounds => WinApi.GetClientRectangle(pM.Process.MainWindowHandle);
    public bool IsPreGame => GameStateActive(AllGameStates[GameStateTypes.PreGameState]);
    public bool IsLoginState => GameStateActive(AllGameStates[GameStateTypes.LoginState]);
    public bool IsSelectCharacterState => GameStateActive(AllGameStates[GameStateTypes.SelectCharacterState]);
    public bool IsWaitingState => GameStateActive(AllGameStates[GameStateTypes.WaitingState]); //This happens after selecting character, maybe other cases
    public bool IsInGameState => GameStateActive(AllGameStates[GameStateTypes.InGameState]); //In game, with selected character
    public bool IsLoadingState => GameStateActive(AllGameStates[GameStateTypes.LoadingState]);
    public bool IsEscapeState => GameStateActive(AllGameStates[GameStateTypes.EscapeState]);
    public AreaLoadingState LoadingState => GetObject<AreaLoadingState>(AllGameStates[GameStateTypes.AreaLoadingState]);
    public IngameStateWrapper IngameState => GetObject<IngameStateWrapper>(AllGameStates[GameStateTypes.InGameState]);
    public bool IsLoading => LoadingState.IsLoading;
    public bool IsMenuOpen => M.Read<bool>(AllGameStates[GameStateTypes.EscapeState] + 0xE0);
    public bool InGame => IngameState.Address != 0 && IngameState.Data.Address != 0 && IngameState.ServerData.Address != 0 && !IsLoading;
    public int AreaChangeCount => M.Read<int>(M.AddressOfProcess + M.BaseOffsets[OffsetsName.AreaChangeCount]);
    public int BlackBarSize => M.Read<int>(M.AddressOfProcess + M.BaseOffsets[OffsetsName.BlackBarSize]);
    public byte[] TerrainRotationSelector => M.ReadBytes(M.AddressOfProcess + M.BaseOffsets[OffsetsName.TerrainRotationSelector], 8);
    public byte[] TerrainRotationHelper => M.ReadBytes(M.AddressOfProcess + M.BaseOffsets[OffsetsName.TerrainRotationHelper], 24);
    public FilesContainerWrapper Files { get; private set; }
    public CoreSettings CoreSettings { get; private set; }
    public new static GameWrapper TheGame { get; private set; }

    private bool GameStateActive(long stateAddress)
    {
        var address = Address + 0x20;
        var start = M.Read<long>(address);

        //var end = Read<long>(address + 0x8);
        var last = M.Read<long>(address + 0x10);

        var length = (int)(last - start);
        var bytes = M.ReadMem(start, length);

        for (var readOffset = 0; readOffset < length; readOffset += 16)
        {
            var pointer = BitConverter.ToInt64(bytes, readOffset);
            if (stateAddress == pointer) return true;
        }

        return false;
    }

    private Dictionary<GameStateTypes, long> ReadStates(long pointer)
    {
        var result = new Dictionary<GameStateTypes, long>();

        GameStateOffsets data = M.Read<GameStateOffsets>(pointer);

        result[0] = data.State0;
        result[(GameStateTypes)1] = data.State1;
        result[(GameStateTypes)2] = data.State2;
        result[(GameStateTypes)3] = data.State3;
        result[(GameStateTypes)4] = data.State4;
        result[(GameStateTypes)5] = data.State5;
        result[(GameStateTypes)6] = data.State6;
        result[(GameStateTypes)7] = data.State7;
        result[(GameStateTypes)8] = data.State8;
        result[(GameStateTypes)9] = data.State9;
        result[(GameStateTypes)10] = data.State10;
        result[(GameStateTypes)11] = data.State11;

        return result;
    }

    private class GameStateHashNode : RemoteMemoryObject
    {
        public GameStateHashNode Previous => ReadObject<GameStateHashNode>(Address);
        public GameStateHashNode Root => ReadObject<GameStateHashNode>(Address + 0x8);
        public GameStateHashNode Next => ReadObject<GameStateHashNode>(Address + 0x10);

        //public readonly byte Unknown;
        public bool IsNull => M.Read<byte>(Address + 0x19) != 0;

        //private readonly byte byte_0;
        //private readonly byte byte_1;
        public string Key => M.ReadNativeString(Address + 0x20);

        //public readonly int Useless;
        public GameState Value1 => ReadObject<GameState>(Address + 0x40);

        //public readonly long Value2;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                pM?.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
