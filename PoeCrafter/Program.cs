using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using PoeCrafter.Crafters;
using PoeHudWrapper;
using PoeLib;
using PoeLib.Settings;
using Bootstrapper = PoeLib.Bootstrapper;

namespace PoeCrafter;

class Program
{
    const int SWP_NOSIZE = 0x0000;

    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    private static readonly IntPtr MyConsole = GetConsoleWindow();

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    static async Task Main(string[] args)
    {
        try
        {
            if (Screen.PrimaryScreen.Bounds.Width == 2560)
            {
                SetWindowPos(MyConsole, 0, 2553, 0, 1000, 300, SWP_NOSIZE);
            }
            if (Screen.PrimaryScreen.Bounds.Width == 1920)
            {
                SetWindowPos(MyConsole, 0, 1911, 0, 900, 300, SWP_NOSIZE);
            }

            var container = Bootstrap();
            container.GetRequiredService<IGameWrapper>().Initialize();
            foreach (var initializable in container.GetServices<IInitializable>())
                initializable.Initialize();

            var crafter = container.GetRequiredService<GlovesCrafter>();

            Console.WriteLine("Press any key to start...");
            Console.ReadLine();

            await crafter.Craft();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.ReadLine();
        }
    }



    private static IServiceProvider Bootstrap()
    {
        // Create the container as usual
        var container = new ServiceCollection();
        container.AddSingleton<PoeSettings>();
        container.AddLogging();
        new Bootstrapper().RegisterServices(container);
        new TradeBotLib.Bootstrapper().RegisterServices(container);
        new PoeHudWrapper.Bootstrapper().RegisterServices(container);
        container.AddSingleton<ManaGearCrafter>();
        container.AddSingleton<JeweledFoilCrafter>();
        container.AddSingleton<VaalRegaliaCrafter>();
        container.AddSingleton<RingCrafter>();
        container.AddSingleton<AssassinsMarkRingCrafter>();
        container.AddSingleton<StaffCrafter>();
        container.AddSingleton<BootCrafter>();
        container.AddSingleton<ViridianJewelCrafter>();
        container.AddSingleton<DeathBowAltRegalCrafter>();
        container.AddSingleton<ZanaWatchstoneCrafter>();
        container.AddSingleton<HelmCrafter>();
        container.AddSingleton<WandCrafter>();
        container.AddSingleton<FlaskCrafter>();
        container.AddSingleton<SwordCrafter>();
        container.AddSingleton<FocusedAmuletCrafter>();
        container.AddSingleton<AmuletCrafter>();
        container.AddSingleton<ArmorCrafter>();
        container.AddSingleton<AstrolabeCrafter>();
        container.AddSingleton<MaceCrafter>();
        container.AddSingleton<GlovesCrafter>();
        container.AddSingleton<QuiverCrafter>();
        container.AddSingleton<IRarityStateMachine, RarityStateMachine>();

        return container.BuildServiceProvider(); ;
    }
}
