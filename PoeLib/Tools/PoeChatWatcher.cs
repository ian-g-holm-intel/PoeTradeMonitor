using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PoeLib.Parsers;

namespace PoeLib.Tools;

public interface IPoeChatWatcher : IHostedService
{
    event PoeChatWatcher.CharacterJoinedAreaEvent JoinedArea;
    event PoeChatWatcher.CharacterLeftAreaEvent LeftArea;
    event PoeChatWatcher.CharacterOutOfLeagueEvent OutOfLeague;
    event PoeChatWatcher.CharacterMessageEvent MessageFromCharacter;
    event PoeChatWatcher.CharacterMessageEvent MessageToCharacter;
    event PoeChatWatcher.AreaChangedEvent AreaChanged;
    event PoeChatWatcher.AutoReplyMessage AutoReplyMessageReceived;
    bool AutoReplyEnabled { get; set; }
}

public class PoeChatWatcher : IPoeChatWatcher
{
    private const string standaloneLogPath = @"C:\Program Files (x86)\Grinding Gear Games\Path of Exile\logs\Client.txt";
    private const string steamLogPath = @"C:\Program Files (x86)\Steam\steamapps\common\Path of Exile\logs\Client.txt";

    private readonly ILogger<PoeChatWatcher> log;
    private readonly IMessageParser messageParser;
    private readonly IChatMessageCache messageCache;

    private CancellationTokenSource ctSource;
    private Task watcherTask;
    
    private readonly Dictionary<string, DateTime> autoreplyTimestamp = new Dictionary<string, DateTime>();
    public delegate void CurrencyTradeOfferEvent(CurrencyTradeOffer offer);
    public delegate void ItemTradeOfferEvent(ItemTradeOffer offer);
    public delegate void CharacterJoinedAreaEvent(string characterName);
    public event CharacterJoinedAreaEvent JoinedArea;
    public delegate void CharacterLeftAreaEvent(string characterName);
    public event CharacterLeftAreaEvent LeftArea;
    public delegate void CharacterOutOfLeagueEvent();
    public event CharacterOutOfLeagueEvent OutOfLeague;
    public delegate void CharacterMessageEvent(CharacterMessage characterMessage);
    public event CharacterMessageEvent MessageFromCharacter;
    public event CharacterMessageEvent MessageToCharacter;
    public delegate void AreaChangedEvent(bool successful);
    public event AreaChangedEvent AreaChanged;
    public delegate void AutoReplyMessage(string charactername, string reply);
    public event AutoReplyMessage AutoReplyMessageReceived;

    public PoeChatWatcher(IMessageParser messageParser, IChatMessageCache messageCache, ILogger<PoeChatWatcher> log)
    {
        this.messageParser = messageParser;
        this.messageCache = messageCache;
        this.log = log;
    }

    public bool AutoReplyEnabled { get; set; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ctSource = new CancellationTokenSource();
        var standaloneLogWriteTime = File.GetLastWriteTime(standaloneLogPath);
        var steamLogWriteTime = File.GetLastWriteTime(steamLogPath);
        var logPath = standaloneLogWriteTime < steamLogWriteTime ? steamLogPath : standaloneLogPath;
        watcherTask = Task.Factory.StartNew(() =>
        {
            if (!File.Exists(logPath)) return;
            try
            {
                using (var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    sr.ReadToEnd();
                    while (!ctSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            string line;
                            if ((line = sr.ReadLine()) != null)
                            {
                                var joined = messageParser.JoinedArea(line);
                                if (!string.IsNullOrEmpty(joined))
                                {
                                    JoinedArea?.Invoke(joined);
                                    continue;
                                }

                                var left = messageParser.LeftArea(line);
                                if (!string.IsNullOrEmpty(left))
                                {
                                    LeftArea?.Invoke(left);
                                    continue;
                                }

                                if (messageParser.ChangedArea(line))
                                {
                                    AreaChanged?.Invoke(true);
                                    continue;
                                }

                                if (messageParser.FailedChangeArea(line))
                                {
                                    AreaChanged?.Invoke(false);
                                    continue;
                                }

                                var isOutOfLeague = messageParser.IsOutOfLeague(line);
                                if (isOutOfLeague)
                                {
                                    OutOfLeague?.Invoke();
                                    continue;
                                }

                                if (messageParser.TryParseIncomingCharacterMessage(line, out var incomingCharacterMessage))
                                {
                                    if (messageParser.IsIgnoredMessage(incomingCharacterMessage.Message))
                                        continue;

                                    if (AutoReplyEnabled && messageParser.TryGetAutoreply(line, out var reply))
                                    {
                                        if (autoreplyTimestamp.ContainsKey(incomingCharacterMessage.Character) && DateTime.Now - autoreplyTimestamp[incomingCharacterMessage.Character] < TimeSpan.FromMinutes(2))
                                            continue;

                                        autoreplyTimestamp[incomingCharacterMessage.Character] = DateTime.Now;
                                        AutoReplyMessageReceived?.Invoke(incomingCharacterMessage.Character, reply);
                                        continue;
                                    }

                                    messageCache.AddMessage(incomingCharacterMessage);
                                    MessageFromCharacter?.Invoke(incomingCharacterMessage);
                                }

                                if (messageParser.TryParseOutgoingCharacterMessage(line, out var outgoingCharacterMessage))
                                {
                                    MessageToCharacter?.Invoke(outgoingCharacterMessage);
                                }
                            }
                            else
                                Thread.Sleep(100);
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            log.LogError("Caught inner exception in PoeChatWacher: {ex}", ex);
                        }
                    }
                    log.LogInformation("WatcherThread exiting");
                }
            }
            catch (OperationCanceledException)
            {
                log.LogInformation("WatcherThread cancelled, exiting");
            }
            catch (Exception ex)
            {
                log.LogError("Caught outer exception in PoeChatWacher: {ex}", ex);
            }
        }, TaskCreationOptions.LongRunning);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        ctSource.Cancel();
        await watcherTask;
    }
}
