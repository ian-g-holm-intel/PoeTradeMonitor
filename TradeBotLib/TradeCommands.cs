using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AutoIt;
using PoeLib;
using PoeLib.Tools;
using RateLimiter;
using Microsoft.Extensions.Logging;
using ComposableAsync;
using Nito.AsyncEx;
using PoeHudWrapper;
using InputSimulatorStandard.Native;
using InputSimulatorStandard;
using PoeHudWrapper.MemoryObjects;
using PoeHudWrapper.Elements.InventoryElements;
using PoeLib.Settings;
using System.Windows.Forms;

namespace TradeBotLib;

public interface ITradeCommands
{
    void Initialize();
    bool ActivatePoe(int maxRetries = 3, int timeoutMs = 2000);
    Task SelectTab(string tabName);
    Task<string> GetItemInfo(Point itemLoc);
    Task RemoveInventoryCurrency(IEnumerable<Currency> currencyToRemove = null, bool isTrade = false, bool selectTab = true);
    Task RemoveInventoryItems(bool shouldDelay = false);
    Task WithdrawCurrency(IEnumerable<Currency> currencies);
    Task AntiAFK();
    Task CloseAllPanels();
    Task<bool> GotoHideout(string characterName, CancellationToken ct = default);
    Task LeaveParty();
    Task<bool> OpenStash(CancellationToken ct = default);
    Task OpenInventory();
    Task OpenCharacterInfo();
    Task OpenPartyScreen();
    Task TradeWith(string characterName);
    Task<bool> AcceptParty(string name = "");
    Task SendMessage(string characterName, string Message, bool ShouldDelay = false);
    Task SendTextCommand(string Command);
    Task InviteToParty(string characterName);
    Task<bool> WaitTradeWindowOpen(string characterName, int timeoutMs, CancellationToken ct = default);
    Task<bool> WaitPartyRequest(string accountName, CancellationToken ct = default);
    Task<bool> WaitAcceptTrade(string characterName, int timeoutMs, CancellationToken ct = default);
    Task<bool> CheckItemReceived(ItemTradeRequest tradeRequest);
    Task WaitForItem(ItemTradeRequest tradeRequest, int timeoutMs, CancellationToken ct = default);
    Task<bool> WaitCompleteTrade(string characterName, int timeoutMs, ItemTradeRequest tradeRequest, CancellationToken ct = default);
    Task<bool> WaitPlayerJoinArea(string characterName, int timeoutMs, CancellationToken ct = default);
    Task RemoveFromParty(string characterName);
    Task<bool> TradeWindowOpen();
    Task<bool> TryClearClipboard();
    Task<string> GetClipboard();
    Task<bool> PlayerLeftParty(string characterName, CancellationToken ct = default);
    void MouseMove(Point point);
    Task SendKeyDown(VirtualKeyCode keyCode, int delayMs = 20);
    Task SendKeyUp(VirtualKeyCode keyCode, int delayMs = 20);
    Task SendKeyPress(VirtualKeyCode keyCode, int delayMs = 20);
    Task LeftClickMouse(Point point, bool shouldDelay = true);
    Task RightClickMouse(Point point, bool shouldDelay = true);
    Task SellAllItems();
    Task OpenChat(CancellationToken ct = default);
    Task CloseChat(VirtualKeyCode closeCode = VirtualKeyCode.RETURN);
    Task MouseOverItems();
    Task<bool> VerifyItemsCorrect(ItemTradeRequest tradeRequest, IEnumerable<NormalInventoryItemWrapper> theirTradeItems);
    Task<bool> WaitForHideout(int timeoutMs);
}

public class TradeCommands : ITradeCommands
{
    private readonly ILogger<TradeCommands> log;
    private readonly PoeSettings settings;
    private readonly IPriceValidator priceValidator;
    private readonly IInputSimulator input;
    private readonly IPoeChatWatcher chatWatcher;
    private readonly IPoeHudWrapper poeHud;
    private readonly TimeLimiter textCommandLimiter;
    private readonly AsyncLock inputLock;
    private readonly Random randomNumber = new Random();
    private bool initialized = false;

    public TradeCommands(PoeSettings settings, IPriceValidator validator, IInputSimulator inputsim, IPoeChatWatcher chat, IPoeHudWrapper poeHudWrapper, ILogger<TradeCommands> log)
    {
        this.settings = settings;

        log.LogDebug($"Settings:\nHideoutName = {settings.HideoutName}\nAccount = {settings.Account}\nWindowsVersion = {settings.WindowsVersion}\nGuiAddress = {settings.GuiAddress}");

        priceValidator = validator;
        input = inputsim;
        chatWatcher = chat;
        poeHud = poeHudWrapper;
        this.log = log;
        textCommandLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMilliseconds(200));
        inputLock = new AsyncLock();
    }

    public void Initialize()
    {
        if (!initialized)
        {
            poeHud.Initialize();
            initialized = true;
        }
    }

    private async Task ReturnToPreviousWindow(Func<Task> action)
    {
        var poeWindow = Win32.GetPoeWindowPointer();
        var currentWindow = Win32.GetCurrentWindow();
        await action();
        if (currentWindow != poeWindow)
        {
            Win32.SendWindowToBack(poeWindow);
            Win32.SetForegroundWindow(currentWindow);
        }
    }

    private async Task HoldKey(VirtualKeyCode key, Func<Task> action, int delayMs = 20)
    {
        using (await inputLock.LockAsync())
        {
            await SendKeyDown(key, delayMs);
            try
            {
                await action();
            }
            finally
            {
                await SendKeyUp(key, delayMs);
            }
        }
    }

    private async Task HoldKeys(VirtualKeyCode[] keys, Func<Task> action, int delayMs = 20)
    {
        using (await inputLock.LockAsync())
        {
            foreach (var key in keys)
                await SendKeyDown(key, delayMs);
            try
            {
                await action();
            }
            finally
            {
                foreach (var key in keys)
                    await SendKeyUp(key, delayMs);
            }
        }
    }

    public bool ActivatePoe(int maxRetries = 3, int timeoutMs = 2000)
    {
        var poeWindow = Win32.GetPoeWindowPointer();
        if (poeWindow == IntPtr.Zero)
        {
            log.LogWarning("PoE window not found");
            return false;
        }

        if (poeWindow == Win32.GetCurrentWindow())
            return true;

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                // Ensure window exists and is valid
                if (!Win32.IsWindow(poeWindow))
                {
                    log.LogWarning("PoE window handle no longer valid");
                    return false;
                }

                Win32.ActivateWindow(poeWindow);

                // Use longer timeout and shorter sleep interval
                if (Win32.WaitWindowActive(poeWindow, TimeSpan.FromMilliseconds(timeoutMs)))
                    return true;

                // Add small delay between retries
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                log.LogError($"Error activating PoE window: {ex.Message}");
                return false;
            }
        }

        log.LogWarning($"Failed to activate PoE window after {maxRetries} attempts");
        return false;
    }

    public void MouseMove(Point point)
    {
        MouseMove(point.X, point.Y);
    }

    private void MouseMove(int x, int y)
    {
        if ((x == 0 && y == 0) || (x == -1 && y == -1))
        {
            log.LogError($"Tried moving mouse to ({x},{y}): {Environment.StackTrace}");
        }

        ActivatePoe();
        var clientBounds = poeHud.ClientBounds;
        var relativeX = x + clientBounds.X;
        input.Mouse.MoveMouseTo(Math.Round(Convert.ToDouble(ushort.MaxValue * relativeX) / clientBounds.Width), Math.Round(Convert.ToDouble(ushort.MaxValue * y) / clientBounds.Height));
    }

    public async Task SendKeyUp(VirtualKeyCode keyCode, int delayMs = 20)
    {
        ActivatePoe();
        input.Keyboard.KeyUp(keyCode);
        await Task.Delay(delayMs);
    }

    public async Task SendKeyDown(VirtualKeyCode keyCode, int delayMs = 20)
    {
        ActivatePoe();
        input.Keyboard.KeyDown(keyCode);
        await Task.Delay(delayMs);
    }

    public async Task SendKeyPress(VirtualKeyCode keyCode, int delayMs = 20)
    {
        ActivatePoe();
        input.Keyboard.KeyPress(keyCode);
        await Task.Delay(delayMs);
    }

    private Task SendText(string text)
    {
        ActivatePoe();
        if (!string.IsNullOrEmpty(poeHud.ChatText))
        {
            input.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
        }
        input.Keyboard.TextEntry(text);
        return Task.CompletedTask;
    }

    public async Task LeftClickMouse(Point point, bool shouldDelay = true)
    {
        await LeftClickMouse(point.X, point.Y, shouldDelay);
    }

    private async Task LeftClickMouse(int x, int y, bool shouldDelay = true)
    {
        ActivatePoe();
        MouseMove(x, y);
        await Task.Delay(30);
        input.Mouse.LeftButtonClick();
        await Task.Delay(shouldDelay ? Convert.ToInt32(poeHud.Latency) : 10);
    }

    public async Task RightClickMouse(Point point, bool shouldDelay = true)
    {
        await RightClickMouse(point.X, point.Y, shouldDelay);
    }

    private async Task RightClickMouse(int x, int y, bool shouldDelay = true)
    {
        ActivatePoe();
        MouseMove(x, y);
        await Task.Delay(50);
        input.Mouse.RightButtonClick();
        await Task.Delay(shouldDelay ? Convert.ToInt32(poeHud.Latency) : 10);
    }

    public async Task SendTextCommand(string Command)
    {
        await textCommandLimiter;
        using (await inputLock.LockAsync())
        {
            await OpenChat();
            await SendText(Command);
            await CloseChat();
        }
    }

    public async Task OpenChat(CancellationToken ct = default)
    {
        if (!await CheckConditionTrue(() => !poeHud.ChatVisible && !poeHud.MenuOpen, 10, 20, ct))
            await CloseChat(VirtualKeyCode.ESCAPE);

        await DoUntilTrueAsync(async () =>
        {
            await SendKeyPress(VirtualKeyCode.RETURN);
            await Task.Delay(20);
        }, () => poeHud.ChatVisible, 5000);
    }

    public async Task CloseChat(VirtualKeyCode closeCode = VirtualKeyCode.RETURN)
    {
        if (!poeHud.ChatVisible)
            return;

        await DoUntilTrueAsync(async () =>
        {
            await SendKeyPress(VirtualKeyCode.RETURN);
            await Task.Delay(20);
        }, () => !poeHud.ChatVisible, 5000);
    }

    public Task<bool> TryClearClipboard()
    {
        return Task.Run(() =>
        {
            try
            {
                AutoItX.ClipPut("");
                return true;
            }
            catch (ExternalException)
            {
                return false;
            }
        });
    }

    public Task<string> GetClipboard()
    {
        return Task.Run(() =>
        {
            try
            {
                return AutoItX.ClipGet();
            }
            catch (ExternalException)
            {
                return "";
            }
        });
    }

    public async Task<string> GetItemInfo(Point itemLoc)
    {
        while (!await TryClearClipboard())
        {
            await Task.Delay(10);
        }

        MouseMove(itemLoc);
        await Task.Delay(30);
        AutoItX.Send("^c");
        await Task.Delay(30);
        return AutoItX.ClipGet();
    }

    private async Task<string> GetFirstTradeItemsInfo()
    {
        try
        {
            var items = poeHud.TheirTradeItems.ToArray();
            if (!items.Any()) return "";
            return await GetItemInfo(items.First().Center);
        }
        finally
        {
            if (!await TradeWindowOpen())
                throw new TradeClosedException();
        }
    }

    public async Task RemoveInventoryCurrency(IEnumerable<Currency> currencyToRemove = null, bool isTrade = false, bool selectTab = true)
    {
        log.LogInformation("Removing inventory currency");

        if (isTrade)
            await Delay(500, 800);

        if (currencyToRemove != null)
        {
            foreach (var currencyType in currencyToRemove)
                log.LogInformation($"Depositing {currencyType} to stash");
        }

        if(selectTab)
            await SelectTab("$");

        var slots = currencyToRemove?.Sum(currencyItem => currencyItem.GetCurrencyStatcks()) ?? 60;
        var columns = slots / 5;
        var remainder = slots % 5;

        var inventoryCurrency = poeHud.PlayerInventoryCurrency;

        Func<Point, bool> isClicked;
        if(isTrade)
            isClicked = (slot) => poeHud.IsInventorySelected(slot) || !poeHud.TradeWindowOpen;
        else
            isClicked = (slot) => !poeHud.PlayerInventoryCurrency.Any(c => c.Slot == slot);

        if (poeHud.ItemOnCursor)
        {
            await ClickInventoryNoDestroy(getFreeItemSlot);
        }

        using (var ctSource = new CancellationTokenSource(30000))
        {
            await HoldKeys([VirtualKeyCode.CONTROL, VirtualKeyCode.SHIFT], async () =>
            {
                if (currencyToRemove == null)
                {
                    foreach (var currency in inventoryCurrency.OrderBy(x => x.Location.X).ThenBy(y => y.Location.Y))
                    {
                        await CheckForPartyInvite(isTrade, inventoryCurrency.Count);
                        while (!ctSource.IsCancellationRequested && poeHud.InventoryPanelOpen && !isClicked(currency.Slot))
                        {
                            await RightClickMouse(currency.Location, isTrade);
                        }
                    }
                }
                else
                {
                    foreach (var currency in currencyToRemove)
                    {
                        var movedAmount = 0m;
                        foreach (var invCurrency in inventoryCurrency.Where(c => c.Type == currency.Type).OrderBy(x => x.Location.X).ThenBy(y => y.Location.Y))
                        {
                            if (invCurrency.Amount + movedAmount > currency.Amount)
                                continue;

                            await CheckForPartyInvite(isTrade, inventoryCurrency.Count);
                            while (!ctSource.IsCancellationRequested && poeHud.InventoryPanelOpen && !isClicked(invCurrency.Slot))
                            {
                                await RightClickMouse(invCurrency.Location, isTrade);
                            }
                            movedAmount += invCurrency.Amount;
                        }
                    }
                }
            });

            if(ctSource.IsCancellationRequested)
                throw new RemoveCurrencyFailedException();

            log.LogInformation("Finished removing currency");
        }
    }

    private async Task CheckForPartyInvite(bool shouldDecline, int inventorySlotCount)
    {
        if (inventorySlotCount <= 25) return;
        var partyInvites = poeHud.PartyInvites;
        foreach (var invite in partyInvites)
        {
            if (shouldDecline)
            {
                await LeftClickMouse(invite.DeclineButtonLocation);
            }
            else
            {
                await LeftClickMouse(invite.AcceptButtonLocation);
            }
        }
    }

    public async Task SellAllItems()
    {
        var items = poeHud.PlayerInventoryItems;
        ActivatePoe();
        foreach (var item in items)
        {
            await MoveItem(item, false, false);
        }
    }

    public async Task RemoveInventoryItems(bool shouldDelay = false)
    {
        if (shouldDelay)
                await Delay(500, 1000);

        try
        {
            var essences = new List<ServerInventoryWrapper.InventSlotItemWrapper>();
            var currency = new List<ServerInventoryWrapper.InventSlotItemWrapper>();
            var fossils = new List<ServerInventoryWrapper.InventSlotItemWrapper>();
            var resonators = new List<ServerInventoryWrapper.InventSlotItemWrapper>();
            var maps = new List<ServerInventoryWrapper.InventSlotItemWrapper>();
            var fragments = new List<ServerInventoryWrapper.InventSlotItemWrapper>();
            var beasts = new List<ServerInventoryWrapper.InventSlotItemWrapper>();
            var remainingItems = new List<ServerInventoryWrapper.InventSlotItemWrapper>();
            var items = poeHud.PlayerInventoryItems;
            foreach (var item in items)
            {
                if(poeHud.IsItemCurrency(item.Item))
                    currency.Add(item);
                else if(poeHud.IsItemEssence(item.Item))
                    essences.Add(item);
                else if(poeHud.IsItemFossil(item.Item))
                    fossils.Add(item);
                else if(poeHud.IsItemResonator(item.Item))
                    resonators.Add(item);
                else if(poeHud.IsItemBeast(item.Item))
                    beasts.Add(item);
                else
                    remainingItems.Add(item);
            }

            foreach(var item in essences)
            {
                await MoveItem(item, shouldDelay, false);
            }

            foreach(var item in fragments)
            {
                await MoveItem(item, shouldDelay, false);
            }

            foreach(var item in maps)
            {
                await MoveItem(item, shouldDelay, false);
            }

            foreach(var item in fossils)
            {
                await MoveItem(item, shouldDelay, false);
            }

            foreach(var item in resonators)
            {
                await MoveItem(item, shouldDelay, false);
            }

            foreach (var item in currency)
            {
                await CheckForPartyInvite(true, currency.Count);
                await MoveItem(item, shouldDelay, false);
            }

			if (beasts.Any())
				await SelectTab("Beasts");
            foreach (var item in beasts)
            {
                await MoveItem(item, shouldDelay);
            }

			if (remainingItems.Any())
                await SelectTab("Sale");
            foreach (var item in remainingItems)
            {
                await MoveItem(item, shouldDelay);
            }
        }
        finally
        {
            await Task.Delay(50);
            if (poeHud.PlayerInventoryCurrency.Any())
                await RemoveInventoryCurrency();
        }
    }

    public async Task SelectTab(string tabName)
    {
        ActivatePoe();
        log.LogInformation($"Switching to tab: {tabName}");
        await poeHud.SwitchToTab(tabName);
    }

    private async Task MoveItem(ServerInventoryWrapper.InventSlotItemWrapper item, bool shouldDelay, bool shiftClick = true)
    {
        await HoldKey(VirtualKeyCode.CONTROL, async () =>
        {
            if (shiftClick)
                await SendKeyDown(VirtualKeyCode.SHIFT);
            await LeftClickMouse(item.Center, shouldDelay);
            if (shiftClick)
                await SendKeyUp(VirtualKeyCode.SHIFT);
        });
        if (shouldDelay)
            await Delay(0, 400);
    }

    public async Task WithdrawCurrency(IEnumerable<Currency> currencies)
    {
        await SelectTab("$");
        foreach (var currency in currencies)
        {
            log.LogInformation($"Withdrawing {currency} from stash");
            var totalRemaining = currency.Amount;
            var currencyStackSize = currency.Type.GetCurrencyStackSize();

            var availableCurrency = poeHud.GetStashCurrencyOfType(currency.Type);
            using (var ctSource = new CancellationTokenSource(5000))
            {
                while (!ctSource.IsCancellationRequested && !availableCurrency.Any())
                {
                    availableCurrency = poeHud.GetStashCurrencyOfType(currency.Type);
                }
            }

            var totalCurrency = availableCurrency.Sum(c => c.Amount);
            if (totalCurrency < currency.Amount)
            {
                throw new InsufficientCurrencyException();
            }

            int getInventoryCurrencyAmount() => Convert.ToInt32(poeHud.PlayerInventoryCurrency.Where(c => c.Type == currency.Type).Sum(c => c.Amount));
            foreach (var currentCurrency in availableCurrency.OrderBy(c => c.Amount))
            {
                if(currentCurrency.Amount < totalRemaining)
                {
                    await MoveCurrency(getInventoryCurrencyAmount, currentCurrency.Location, Convert.ToInt32(currentCurrency.Amount));
                    totalRemaining -= currentCurrency.Amount;
                }
                else
                {
                    int totalStacks = 0;
                    var stacks = (int)totalRemaining / currencyStackSize;
                    totalStacks += stacks;
                    var remaining = (int)totalRemaining % currencyStackSize;
                    if (remaining != 0)
                        totalStacks++;

                    if (stacks != 0)
                    {
                        await MoveCurrency(getInventoryCurrencyAmount, currentCurrency.Location, stacks * currencyStackSize);
                        currentCurrency.Amount -= stacks * currencyStackSize;
                    }

                    if (remaining != 0)
                    {
                        if (currentCurrency.Amount == 1)
                        {
                            await HoldKey(VirtualKeyCode.CONTROL, () => LeftClickMouse(currentCurrency.Location));
                        }
                        else
                        {
                            await CheckForPartyInvite(true, Convert.ToInt32(currency.Amount) / currencyStackSize);
                            await HoldKey(VirtualKeyCode.SHIFT, () => LeftClickMouse(currentCurrency.Location));
                            AutoItX.Send(remaining.ToString());
                            await Task.Delay(20);
                            await SendKeyPress(VirtualKeyCode.RETURN);
                            await DoUntilTrueAsync(() => Task.Delay(20), () => poeHud.ItemOnCursor && !poeHud.ChatVisible, 5000);

                            var partialStack = poeHud.PlayerInventoryCurrency.Where(c => c.Type == currency.Type).FirstOrDefault(c => c.Amount < currencyStackSize);
                            if (partialStack != null)
                            {
                                var partialStackAmount = partialStack.Amount;
                                await ClickInventoryNoDestroy(() => partialStack.Slot, partialStackAmount + remaining > currencyStackSize);

                                if (partialStackAmount + remaining > currencyStackSize)
                                {
                                    await ClickInventoryNoDestroy(getFreeItemSlot);
                                }
                            }
                            else
                            {
                                await ClickInventoryNoDestroy(getFreeItemSlot);
                            }
                        }
                    }
                    break;
                }
            }
        }
    }

    private Point getFreeItemSlot()
    {
        var freeSlot = poeHud.FreeItemSlot;
        if (freeSlot == new Point(-1, -1))
            throw new FreeItemSlotException();
        return freeSlot;
    }

    private async Task MoveCurrency(Func<int> getInventoryCurrencyAmount, Point currencyLocation, int amount)
    {
        using (var moveCurrencyCts = new CancellationTokenSource(30000))
        {
            await HoldKey(VirtualKeyCode.CONTROL, async () =>
            {
                var initialAmount = getInventoryCurrencyAmount();
                var currentAmount = initialAmount;
                while (!moveCurrencyCts.IsCancellationRequested && currentAmount < initialAmount + amount)
                {
                    await LeftClickMouse(currencyLocation);

                    using (var ctSource = new CancellationTokenSource(1000))
                    {
                        while (!ctSource.IsCancellationRequested && getInventoryCurrencyAmount() == currentAmount)
                        {
                            await Task.Delay(20);
                        }
                    }
                    currentAmount = getInventoryCurrencyAmount();
                }
            });
        }
    }

    private async Task ClickInventoryNoDestroy(Func<Point> getPoint, bool finishItemOnCursor = false)
    {
        await DoUntilTrueAsync(async () =>
        {
            if (poeHud.DestroyConfirmationVisible)
            {
                var keepButtonLocation = poeHud.DestroyConfirmationKeepButtonLocation;
                await LeftClickMouse(keepButtonLocation);
                if (!poeHud.InventoryPanelOpen)
                    await OpenInventory();
            }

            await LeftClickMouse(poeHud.GetLocationFromInventorySlot(getPoint()));
        }, () => (!finishItemOnCursor && !poeHud.ItemOnCursor || finishItemOnCursor && poeHud.ItemOnCursor) || !poeHud.StashOpen, 30000);
    }

    public async Task AntiAFK()
    {
        while (Control.ModifierKeys != Keys.None)
            await Task.Delay(10);

        await ReturnToPreviousWindow(() => SendTextCommand("/played"));
    }

    public async Task CloseAllPanels()
    {
        if (poeHud.TradeInviteSentVisible)
            await SendKeyPress(VirtualKeyCode.ESCAPE);

        await SendKeyPress(VirtualKeyCode.F2);
        await Task.Delay(50);
    }

    public async Task<bool> GotoHideout(string characterName, CancellationToken ct = default)
    {
        if(string.IsNullOrEmpty(characterName))
            log.LogInformation("Going to hideout");
        else if (poeHud.GetPartyMemberZone(characterName) == "Kingsmarch")
            log.LogInformation("Going to kingsmarch of {characterName}", characterName);

        await SendKeyPress(VirtualKeyCode.F2);

        var retryCount = 5;
        if (poeHud.GetPartyMemberZone(characterName) == "Kingsmarch")
        {
            if (!await GotoPlayersKingsmarch(characterName, retryCount, ct))
            {
                log.LogError($"Failed to join kingsmarch");
                return false;
            }
        }
        else
        {
            if (!await GotoPlayersHideout(characterName, retryCount, ct))
            {
                log.LogError($"Failed to join hideout");

                if (poeHud.IsLoading)
                    await Task.Delay(100);

                if (!poeHud.InGame)
                {
                    await SendKeyPress(VirtualKeyCode.RETURN);
                    while (!poeHud.InGame)
                        await Task.Delay(100);
                }

                return false;
            }
        }
        return true;
    }

    private async Task<bool> GotoPlayersHideout(string characterName, int retryCount, CancellationToken ct = default)
    {
        var areaChangeSuccessful = new AsyncManualResetEvent(false);
        void OnAreaChanged(bool successful)
        {
            if (successful)
                areaChangeSuccessful.Set();
        }

        chatWatcher.AreaChanged += OnAreaChanged;

        try
        {
            for (int i = 0; i < retryCount; i++)
            {
                await SendTextCommand("/hideout " + characterName);

                if (!await CheckConditionTrue(() => poeHud.IsLoading, 30, 100))
                {
                    continue;
                }

                await areaChangeSuccessful.WaitAsync(ct);

                if (!string.IsNullOrEmpty(characterName) && await PlayerLeftParty(characterName))
                {
                    log.LogError("{characterName} left the party", characterName);
                    return false;
                }

                return true;
            }
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        finally
        {
            chatWatcher.AreaChanged -= OnAreaChanged;
        }
    }

    private async Task<bool> GotoPlayersKingsmarch(string characterName, int retryCount, CancellationToken ct = default)
    {
        var areaChangeSuccessful = new AsyncManualResetEvent(false);
        void OnAreaChanged(bool successful)
        {
            if (successful)
                areaChangeSuccessful.Set();
        }

        chatWatcher.AreaChanged += OnAreaChanged;

        try
        {
            for (int i = 0; i < retryCount; i++)
            {
                var buttonLocation = poeHud.PartyMemberPortalButtonLocation(characterName);
                await LeftClickMouse(buttonLocation);

                if (!await CheckConditionTrue(() => poeHud.IsLoading, 30, 100))
                {
                    continue;
                }

                await areaChangeSuccessful.WaitAsync(ct);

                if (!string.IsNullOrEmpty(characterName) && await PlayerLeftParty(characterName))
                {
                    log.LogError("{characterName} left the party", characterName);
                    return false;
                }

                return true;
            }
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        finally
        {
            chatWatcher.AreaChanged -= OnAreaChanged;
        }
    }

    public async Task<bool> OpenStash(CancellationToken ct = default)
    {
        ActivatePoe();
        if (!await CheckConditionTrue(() => poeHud.AreaName == settings.HideoutName, 100, 20, ct))
            await GotoHideout("", ct);

        using var ctSource = new CancellationTokenSource(30000);
        using var linkedCtSource = CancellationTokenSource.CreateLinkedTokenSource(ctSource.Token, ct);
        try
        {
            while (!linkedCtSource.Token.IsCancellationRequested)
            {
                if (poeHud.StashOpen)
                {
                    log.LogInformation("Stash opened");
                    return true;
                }

                await DoUntilTrueAsync(async () =>
                {
                    var stashLocation = poeHud.StashLocation;
                    await LeftClickMouse(stashLocation.X, stashLocation.Y);
                }, () => poeHud.StashOpen, 3000);
            }
            return false;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to open stash");
            return false;
        }
    }

    public async Task OpenCharacterInfo()
    {
        await SendKeyPress(VirtualKeyCode.VK_C);
        await Task.Delay(50);
    }

    public async Task OpenPartyScreen()
    {
        ActivatePoe();
        if (!poeHud.SocialPanelOpen)
        {
            await DoUntilTrueAsync(async () =>
            {
                await SendKeyPress(VirtualKeyCode.VK_S);
                await Task.Delay(100);
            }, () => poeHud.SocialPanelOpen, 5000);
        }

        if (!poeHud.PartyTabVisible)
        {
            await DoUntilTrueAsync(async () =>
            {
                await LeftClickMouse(poeHud.PartyTabLocation.X, poeHud.PartyTabLocation.Y);
                await Task.Delay(100);
            }, () => poeHud.PartyTabVisible, 5000);
        }
    }

    public async Task TradeWith(string characterName)
    {
        var tradeInvite = poeHud.TradeInvites.SingleOrDefault(invite => invite.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase));
        if (tradeInvite != null)
        {
            await Task.Delay(1000);
            await LeftClickMouse(tradeInvite.AcceptButtonLocation);
            return;
        }
        else
        {
            await Task.Delay(1000);
            await SendTextCommand("/tradewith " + characterName);
        }
    }

    private async Task<bool> AcceptTrade(ItemTradeRequest tradeRequest, int timeoutMs)
    {
        log.LogInformation("Accepting trade");
        await Task.Delay(500);

        if (!await VerifyItemsCorrect(tradeRequest, poeHud.TheirTradeItems))
            return false;

        return await DoUntilTrueAsync(async () =>
        {
            await MouseOverItems();
            await LeftClickMouse(poeHud.AcceptTradeButtonLocation.X, poeHud.AcceptTradeButtonLocation.Y);
        }, () => poeHud.AcceptButtonClicked || !poeHud.TradeWindowOpen, timeoutMs);
    }

    public async Task<bool> AcceptParty(string name = "")
    {
        if (poeHud.PlayerInParty())
        {
            log.LogInformation("Already in party");
            return true;
        }

        await OpenPartyScreen();

        var pendingInvites = poeHud.PendingInvites;
        var acceptButtonLocation = poeHud.GetPartyInviteAcceptLocation(pendingInvites.Contains(name) ? name : "");
        if (acceptButtonLocation == new Point(-1, -1))
            return false;
        await LeftClickMouse(acceptButtonLocation.X, acceptButtonLocation.Y);
        return true;
}

    public async Task LeaveParty()
    {
        var inParty = poeHud.PlayerInParty();
        log.LogInformation($"In party: {inParty}");

        if (inParty)
        {
            log.LogInformation("Leaving party");
            await SendTextCommand("/leave");
        }
    }

    public async Task SendMessage(string characterName, string Message, bool ShouldDelay = false)
    {
        await ReturnToPreviousWindow(async () =>
        {
            if (ShouldDelay)
                await Task.Delay(randomNumber.Next(1500, 2500));

            await SendTextCommand($"@{characterName} {Message}");
        });
    }

    private async Task Delay(int min, int max)
    {
        var random = new Random();
        await Task.Delay(random.Next(min, max));
    }

    public async Task OpenInventory()
    {
        if (poeHud.InventoryPanelOpen)
            return;

        await DoUntilTrueAsync(() => SendKeyPress(VirtualKeyCode.VK_I), () => poeHud.InventoryPanelOpen, 5000);

        await Task.Delay(200);
    }

    public async Task InviteToParty(string characterName)
    {
        await SendTextCommand("/invite " + characterName);
    }

    public async Task<bool> PlayerLeftParty(string characterName, CancellationToken ct = default)
    {
        if(!await CheckConditionTrue(() => poeHud.PlayerInParty(characterName), 100, 20, ct))
        {
            log.LogInformation("Player left party");
            return true;
        }
        return false;
    }

    public Task<bool> WaitPlayerJoinArea(string characterName, int timeoutMs, CancellationToken ct = default)
    {
        return Task.Run(async () =>
        {
            var stopwatch = Stopwatch.StartNew();
            while (!ct.IsCancellationRequested && stopwatch.ElapsedMilliseconds < timeoutMs)
            {
                if (poeHud.PlayerInZone(characterName))
                    return true;

                if(await PlayerLeftParty(characterName))
                {
                    break;
                }

                await Task.Delay(100, ct);
            }

            log.LogInformation($"Players in area: {poeHud.CharactersInArea.Aggregate("", (current, player) => current + player + ", ").TrimEnd(',', ' ')}");
            return false;
        });
    }

    public Task<bool> TradeWindowOpen()
    {
        return Task.FromResult(poeHud.TradeWindowOpen);
    }

    public Task<bool> WaitTradeWindowOpen(string characterName, int timeoutMs, CancellationToken ct = default)
    {
        return Task.Run(async () =>
        {
            using (await inputLock.LockAsync())
            {
                var stopwatch = Stopwatch.StartNew();
                var acceptButtonLocation = new Point(-1, -1);
                while (!ct.IsCancellationRequested && stopwatch.ElapsedMilliseconds < timeoutMs)
                {
                    if (poeHud.TradeWindowOpen)
                    {
                        await Task.Delay(randomNumber.Next(500, 800), ct);
                        return true;
                    }

                    var currentAcceptButtonLocation = poeHud.TradeInvites.SingleOrDefault(invite => invite.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase))?.AcceptButtonLocation ?? new Point(-1, -1);
                    if (currentAcceptButtonLocation != new Point(-1, -1))
                    {
                        if (currentAcceptButtonLocation != acceptButtonLocation)
                        {
                            acceptButtonLocation = currentAcceptButtonLocation;
                        }
                        else
                        {
                            await Task.Delay(randomNumber.Next(200, 400), ct);
                            await LeftClickMouse(currentAcceptButtonLocation);
                            return true;
                        }
                    }

                    if (await PlayerLeftParty(characterName))
                        return false;

                    await Task.Delay(100, ct);
                }

                return false;
            }
        });
    }

    public Task<bool> WaitPartyRequest(string accountName, CancellationToken ct = default)
    {
        return Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                if (!poeHud.InGame)
                {
                    await Task.Delay(1000, ct);
                }
                else
                {
                    var partyInvite = poeHud.PartyInvites.SingleOrDefault(invite => invite.AccountName.Equals(accountName, StringComparison.OrdinalIgnoreCase));
                    if (partyInvite != null)
                        return true;
                    await Task.Delay(100, ct);
                }
            }

            return false;
        }, ct);
    }

    public Task<bool> WaitAcceptTrade(string characterName, int timeoutMs, CancellationToken ct = default)
    {
        return Task.Run(async () =>
        {
            var stopwatch = Stopwatch.StartNew();
            while (!ct.IsCancellationRequested && stopwatch.ElapsedMilliseconds < timeoutMs)
            {
                if (poeHud.AcceptButtonClickable)
                    return true;

                if (await PlayerLeftParty(characterName))
                    return false;
            }

            return false;
        });
    }

    public async Task<bool> CheckItemReceived(ItemTradeRequest tradeRequest)
    {
        log.LogInformation($"Checking inventory for {tradeRequest.Item}");
        await OpenInventory();
        try
        {
            var totalItemCount = 0;
            var inventoryItems = poeHud.PlayerInventoryItems;
            for(int i = 0; i < 50; i++)
            {
                if(inventoryItems.Any(inv => inv.Item.IsValid && poeHud.GetItemName(inv.Item).Equals(tradeRequest.Item.Name)))
                    break;

                await Task.Delay(100);
                inventoryItems = poeHud.PlayerInventoryItems;
            }

            foreach (var invItem in poeHud.PlayerInventoryItems)
            {
                var item = invItem.Item;
                await DoUntilTrueAsync(() =>
                {
                    var itemElement = poeHud.GetItemElementFromEntity(item);
                    var itemCenter = itemElement?.Center ?? Point.Empty;
                    if (itemCenter != Point.Empty)
                        MouseMove(itemElement.Center);
                    return Task.CompletedTask;
                }, () => poeHud.IsItemHovered, 1000);
                if (priceValidator.IsCorrectItem(tradeRequest, poeHud.GetItemName(item), item, out var stackSize))
                {
                    totalItemCount += stackSize;
                }
            }

            if(totalItemCount == (tradeRequest.Item.stackSize))
            { 
                return true;
            }
            else
            { 
                log.LogInformation($"Did not find item {tradeRequest.Item} in inventory. Inventory count: {poeHud.PlayerInventoryItems.Count}");
                return false;
            }
        }
        finally
        {
            await CloseAllPanels();
        }
    }

    public async Task WaitForItem(ItemTradeRequest tradeRequest, int timeoutMs, CancellationToken ct = default)
    {
        log.LogInformation($"Waiting for {tradeRequest.Item.Name} to be put in the trade window");
        var timeoutToken = new CancellationTokenSource(timeoutMs);
        while (!timeoutToken.Token.IsCancellationRequested)
        {
            if (!await CheckConditionTrue(() => poeHud.TradeWindowOpen, 5, 20, ct))
            {
                throw new TradeClosedException();
            }

            var theirTradeItems = poeHud.TheirTradeItems.ToList();
            if (theirTradeItems.Any())
            {
                if(tradeRequest.Item.stackSize > 1)
                {
                    if (await VerifyItemsCorrect(tradeRequest, theirTradeItems))
                    {
                        return;
                    }
                }
                else
                {
                    if (await VerifyItemsCorrect(tradeRequest, theirTradeItems))
                        return;
                    else
                        throw new WrongItemException(await GetFirstTradeItemsInfo());
                }    
            }

            await Task.Delay(100);
        }
        throw new TradeTimeoutException();
    }

    public async Task<bool> VerifyItemsCorrect(ItemTradeRequest tradeRequest, IEnumerable<NormalInventoryItemWrapper> theirTradeItems)
    {
        log.LogInformation("Verifying items are correct");
        var totalItemCount = 0;
        foreach (var item in theirTradeItems)
        {
            if(item == null) continue;
            await DoUntilTrueAsync(() =>
            {
                MouseMove(item.Center);
                return Task.CompletedTask;
            }, () => poeHud.IsItemHovered, 1000);
            if (priceValidator.IsCorrectItem(tradeRequest, poeHud.GetItemName(item.Item), item.Item, out var stackSize))
            {
                totalItemCount += stackSize;
            }
        }

        if(totalItemCount != tradeRequest.Item.stackSize)
        {
            log.LogError($"Expected stack size: {tradeRequest.Item.stackSize}, Actual stack size: {totalItemCount}");
        }

        return totalItemCount == tradeRequest.Item.stackSize;
    }

    public async Task MouseOverItems()
    {
        while (poeHud.TheirTradeItems.Any(i => i.IsSelected))
        {
            foreach (var item in poeHud.TheirTradeItems.Where(i => i.IsSelected))
            {
                if (item == null) return;
                MouseMove(item.Center);
                await Task.Delay(20);
            }
        }
    }

    public Task<bool> WaitCompleteTrade(string characterName, int timeoutMs, ItemTradeRequest tradeRequest, CancellationToken ct = default)
    {
        return Task.Run(async () =>
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var tradeComplete = false;
                while (stopwatch.ElapsedMilliseconds < timeoutMs)
                {
                    if (!await CheckConditionTrue(() => poeHud.TradeWindowOpen, 5, 20, ct))
                    {
                        log.LogInformation("Trade window closed");
                        return tradeComplete;
                    }
                        
                    if(!tradeComplete)
                        tradeComplete = await AcceptTrade(tradeRequest, timeoutMs);
                    else if (!poeHud.AcceptButtonClicked)
                        tradeComplete = false;
                    
                    await Task.Delay(100, ct);
                }

                return false;
            }
            catch (TradeFailureException ex)
            {
                log.LogInformation(ex.Message);
                return false;
            }
        });
    }

    private async Task<bool> CheckConditionTrue(Func<bool> condition, int retryCount, int interval, CancellationToken ct = default)
    {
        for (int i = 0; i < retryCount; i++)
        {
            ct.ThrowIfCancellationRequested();
            if (condition())
                return true;
            await Task.Delay(interval, ct);
        }
        return condition();
    }

    public async Task RemoveFromParty(string characterName)
    {
        await Delay(1000, 2000);
        await SendTextCommand("/kick " + characterName);
    }

    private async Task DoWithTimeout(Func<CancellationToken,Task> action, int timeoutMs)
    {
        using(var ctSource = new CancellationTokenSource(timeoutMs))
        {
            await action.Invoke(ctSource.Token);
        }
    }

    private async Task<bool> DoUntilTrueAsync(Func<Task> action, Func<bool> condition, int timeoutMs)
    {
        await action();
        var intervalWatch = Stopwatch.StartNew();
        var timeoutWatch = Stopwatch.StartNew();

        await Task.Delay(100);

        while (!condition() && timeoutWatch.ElapsedMilliseconds < timeoutMs)
        {
            if (intervalWatch.Elapsed > TimeSpan.FromMilliseconds(1000))
            {
                await action();
                intervalWatch.Restart();
            }
            await Task.Delay(50);
        }
        await Task.Delay(50);
        return condition();
    }
    public async Task<bool> WaitForHideout(int timeoutMs)
    {
        log.LogInformation("Waiting to go to hideout");
        using var ctSource = new CancellationTokenSource(timeoutMs);
        while (!ctSource.IsCancellationRequested)
        {
            if (poeHud.AreaName == settings.HideoutName)
                return true;
            await Task.Delay(100);
        }

        return !ctSource.IsCancellationRequested;
    }
}
