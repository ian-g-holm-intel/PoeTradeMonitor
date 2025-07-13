using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Nodes;
using Serilog;

namespace TradeBotLib;

public class ExileCore : IDisposable
{
    public static object SyncLocker = new object();
    private readonly CoreSettings _coreSettings;
    private readonly WaitTime _mainControl = new WaitTime(2000);
    private readonly WaitTime _mainControl2 = new WaitTime(250);
    private readonly SettingsContainer _settings;
    private readonly SoundController _soundController;
    private Memory _memory;
    private bool _memoryValid = true;
    private Rectangle lastClientBound;

    public ExileCore()
    {
        try
        {
            Core.Logger = Log.Logger;
            PerformanceTimer.Logger = new LoggerConfiguration().CreateLogger();
            _settings = new SettingsContainer();
            _coreSettings = _settings.CoreSettings;
            _coreSettings.PerformanceSettings.Threads = new RangeNode<int>(_coreSettings.PerformanceSettings.Threads.Value, 0, Environment.ProcessorCount);
            CoroutineRunner = new Runner("Main Coroutine");
            CoroutineRunnerParallel = new Runner("Parallel Coroutine");

            _soundController = new SoundController("Sounds");
            _coreSettings.Volume.OnValueChanged += (sender, i) => { _soundController.SetVolume(i / 100f); };

            Core.MainRunner = CoroutineRunner;
            Core.ParallelRunner = CoroutineRunnerParallel;

            // Task.Run(ParallelCoroutineRunner);
            var th = new Thread(ParallelCoroutineManualThread) { Name = "Parallel Coroutine", IsBackground = true };
            th.Start();

            MultiThreadManager = new MultiThreadManager(_coreSettings.PerformanceSettings.Threads);
            CoroutineRunner.MultiThreadManager = MultiThreadManager;

            _coreSettings.PerformanceSettings.Threads.OnValueChanged += (sender, i) =>
            {
                if (MultiThreadManager == null)
                    MultiThreadManager = new MultiThreadManager(i);
                else
                {
                    var coroutine1 =
                        new Coroutine(() => { MultiThreadManager.ChangeNumberThreads(_coreSettings.PerformanceSettings.Threads); },
                            new WaitTime(2000), null, "Change Threads Number", false)
                        { SyncModWork = true };

                    Core.ParallelRunner.Run(coroutine1);
                }
            };

            if (_memory == null) _memory = FindPoe();

            if (GameController == null && _memory != null) Inject();

            var coroutine = new Coroutine(MainControl(), null, "Render control") { Priority = CoroutinePriority.Critical };
            CoroutineRunnerParallel.Run(coroutine);

            Task.Run(async () =>
            {
                while (true)
                {
                    Tick();
                    TickCoroutines();
                    WaitRender.Frame();
                    await Task.Delay(10);
                }
            });
        }
        catch (Exception e)
        {
            Core.Logger.Error($"Core constructor -> {e}");
        }
    }

    public MultiThreadManager MultiThreadManager { get; private set; }
    public Runner CoroutineRunner { get; set; }
    public Runner CoroutineRunnerParallel { get; set; }
    public GameController GameController { get; private set; }
    public bool GameStarted { get; private set; }
    public bool IsForeground { get; private set; }
    public Rectangle ClientBounds => WinApi.GetClientRectangle(_memory.Process.MainWindowHandle);

    public void Dispose()
    {
        _memory?.Dispose();
        GameController?.Dispose();
    }

    private IEnumerator MainControl()
    {
        while (true)
        {
            if (_memory == null)
            {
                _memory = FindPoe();
                if (_memory == null) yield return _mainControl;
                continue;
            }

            if (GameController == null && _memory != null)
            {
                Inject();
                if (GameController == null) yield return _mainControl;
                continue;
            }

            var clientRectangle = WinApi.GetClientRectangle(_memory.Process.MainWindowHandle);

            if (lastClientBound != clientRectangle && clientRectangle.Width > 2 && clientRectangle.Height > 2)
            {
                lastClientBound = clientRectangle;
            }

            _memoryValid = !_memory.IsInvalid();

            if (!_memoryValid)
            {
                GameController.Dispose();
                GameController = null;
                _memory = null;
            }
            else
            {
                var isForegroundWindow = WinApi.IsForegroundWindow(_memory.Process.MainWindowHandle) || _coreSettings.ForceForeground;

                IsForeground = isForegroundWindow;
                GameController.IsForeGroundCache = isForegroundWindow;
            }

            yield return _mainControl2;
        }
    }

    public Memory FindPoe()
    {
        var processData = FindPoeProcess();

        if (processData is null or { process.Id: 0 })
            DebugWindow.LogMsg("Game not found");
        else
            return new Memory(processData.Value.process, processData.Value.offsets, _coreSettings);

        return null;
    }

    private void Inject()
    {
        try
        {
            if (_memory != null)
            {
                GameController = new GameController(_memory, _soundController, _settings, MultiThreadManager);
            }
        }
        catch (Exception e)
        {
            DebugWindow.LogError($"Inject -> {e}");
        }
    }

    private void LostFocus(object sender, EventArgs eventArgs)
    {
        if (!WinApi.IsIconic(_memory.Process.MainWindowHandle))
            WinApi.SetForegroundWindow(_memory.Process.MainWindowHandle);
    }

    public void Tick()
    {
        try
        {
            Core.FramesCount++;
            GameController?.Tick();
        }
        catch (Exception ex)
        {
            DebugWindow.LogError($"Core tick -> {ex}");
        }
    }

    private static (Process process, Offsets offsets)? FindPoeProcess()
    {
        var clients = new[] { Offsets.Regular, Offsets.Korean, Offsets.Steam, Offsets.Epic, }
            .SelectMany(o => Process.GetProcessesByName(o.ExeName).Select(p => (p, o)))
            .Where(x =>
            {
                var pId = -1;
                try
                {
                    pId = x.p.Id;
                    return !x.p.HasExited;
                }
                catch (Exception ex)
                {
                    DebugWindow.LogError($"Unable to access process {pId}: {ex}");
                    return false;
                }
            })
            .OrderBy(x => x.p.Id)
            .ToList();
        if (!clients.Any())
        {
            return null;
        }

        var ixChosen = clients.Count > 1 ? ProcessPicker.ShowDialogBox(clients.Select(x => x.p)) : 0;
        switch (ixChosen)
        {
            case null:
                {
                    Environment.Exit(0);
                    return null;
                }
            case -1:
                return null;
            default:
                return clients[ixChosen.Value];
        }
    }

    private void ParallelCoroutineManualThread()
    {
        try
        {
            while (true)
            {
                MultiThreadManager?.Process(this);
                if (CoroutineRunnerParallel.IsRunning)
                {
                    try
                    {
                        for (var i = 0; i < CoroutineRunnerParallel.IterationPerFrame; i++)
                        {
                            CoroutineRunnerParallel.Update();
                        }
                    }
                    catch (Exception e)
                    {
                        Core.Logger.Error($"Coroutine Parallel error: {e.Message}", 6, Color.White);
                    }
                }

                Thread.Sleep(10);
            }
        }
        catch (Exception e)
        {
            Core.Logger.Error($"Coroutine Parallel error: {e.Message}", 6, Color.White);
            throw;
        }
    }

    public void TickCoroutines()
    {
        if (CoroutineRunner.IsRunning)
        {
            if (_coreSettings.PerformanceSettings.CoroutineMultiThreading)
                CoroutineRunner.ParallelUpdate();
            else
                CoroutineRunner.Update();
        }
    }
}