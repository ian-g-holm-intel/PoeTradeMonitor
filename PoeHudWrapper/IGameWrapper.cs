using ExileCore.PoEMemory.MemoryObjects;
using PoeHudWrapper.MemoryObjects;
using System.Drawing;

namespace PoeHudWrapper;

public interface IGameWrapper : IDisposable
{
    void Initialize();
    Rectangle ClientBounds { get; }
    bool InGame { get; }
    bool IsLoading { get; }
    FilesContainerWrapper Files { get; }
    IngameStateWrapper IngameState { get; }
    bool IsEscapeState { get; }
    bool IsInGameState { get; }
    bool IsLoadingState { get; }
    bool IsLoginState { get; }
    bool IsPreGame { get; }
    bool IsMenuOpen { get; }
    bool IsSelectCharacterState { get; }
    bool IsWaitingState { get; }
    AreaLoadingState LoadingState { get; }
}