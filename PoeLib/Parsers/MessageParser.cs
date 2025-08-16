using PoeLib.Common;
using System.Text.RegularExpressions;

namespace PoeLib.Parsers;

/// <summary>
/// Interface for parsing Path of Exile chat messages and game events.
/// </summary>
public interface IMessageParser
{
    bool IsOutOfLeague(string message);
    bool TryParseIncomingCharacterMessage(string line, out CharacterMessage? characterMessage);
    bool TryParseOutgoingCharacterMessage(string line, out CharacterMessage? characterMessage);
    bool IsGeneralMessage(string line, out string character, out string message);
    bool IsFreeMaster(string line, out string master, out string character);
    string JoinedArea(string message);
    string LeftArea(string message);
    bool ChangedArea(string message);
    bool FailedChangeArea(string message);
    bool IsIgnoredMessage(string message);
    bool TryGetAutoreply(string message, out string reply);
}

/// <summary>
/// Parses Path of Exile chat log messages to extract game events and player communications.
/// </summary>
public class MessageParser : IMessageParser
{
    private readonly Regex incomingMessagePattern = new Regex(@"@From(?: <[^>]*>)? (?<characterName>[^:]+): (?<message>.*)", RegexOptions.Compiled);
	private readonly Regex outgoingMessagePattern = new Regex(@"@To(?: <[^>]*>)? (?<characterName>[^:]+): (?<message>.*)", RegexOptions.Compiled);
    private readonly Regex generalMessageNamePattern = new Regex(@"(?<=#)(<[^<>]+> )?[^ ]+(?=:)", RegexOptions.Compiled);
    private readonly Regex tradeMessageNamePattern = new Regex(@"(?<=\$)(<[^<>]+> )?[^ ]+(?=:)", RegexOptions.Compiled);
    private readonly Regex guildNamePattern = new Regex(@"<[^<>]+> ", RegexOptions.Compiled);
    private readonly Regex messagePattern = new Regex(@"(?<=: ).+", RegexOptions.Compiled);
    private readonly Regex joinedAreaPattern = new Regex(@"[^ ]+(?= has joined the area)", RegexOptions.Compiled);
    private readonly Regex leftAreaPattern = new Regex(@"[^ ]+(?= has left the area)", RegexOptions.Compiled);
    private readonly Regex changedAreaPattern = new Regex(@"Generating level [\d]+ area", RegexOptions.Compiled);
    private readonly Regex failedChangeAreaPattern = new Regex(@"Failed to join", RegexOptions.Compiled);
    private readonly Regex outOfLeaguePattern = new Regex(@"That character is out of your league", RegexOptions.Compiled);
    private static readonly HashSet<string> ignoredEquals = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "ready", "sorry", "thank you", "ty", "tx", "t4t", "thanks", "one sec", "one min",
        "thankyou", "thx", "gl hf", "glhf", "gl", "dnd", "autoreply", "sold", "a sec", "1 sec", "a min"
    };
    
    private static readonly HashSet<string> ignoredContains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "discord", "www", "afk"
    }; 
    private readonly Dictionary<string, string> autoreplyMessages = new Dictionary<string, string>{ { "how many", "just one" }, { "want both", "just one" }, {"how much", "just one" }, {"both?", "just one" }, {"all?", "just one" }, {"1?", "ya"}, { "one?", "ya" }, { "2", "just one" }, { "3", "just one" }, { "4", "just one" }, { "5", "just one" }, { "6", "just one" }, { "7", "just one" }, { "8", "just one" }, { "9", "just one" }, { "still interested?", "no thanks" }, {"still need", "no thanks"} };

    public bool TryGetAutoreply(string message, out string reply)
    {
        reply = "";
        foreach (var msg in autoreplyMessages.Keys)
        {
            if (!message.ToLower().Replace(" ?", "?").Contains(msg)) continue;
            reply = autoreplyMessages[msg];
            return true;
        }

        return false;
    }

    public bool IsOutOfLeague(string message)
    {
        return outOfLeaguePattern.IsMatch(message);
    }

    /// <summary>
    /// Determines if a message should be ignored based on common phrases and content.
    /// </summary>
    public bool IsIgnoredMessage(string message)
    {
        if (message.Contains("Hi, I would like to buy", StringComparison.OrdinalIgnoreCase)) 
            return false;
            
        return ignoredContains.Any(ignored => message.Contains(ignored, StringComparison.OrdinalIgnoreCase)) ||
               ignoredEquals.Contains(message);
    }

    public bool TryParseIncomingCharacterMessage(string line, out CharacterMessage? characterMessage)
    {
        characterMessage = null;
        var incomingMessageMatch = incomingMessagePattern.Match(line);
        if (!incomingMessageMatch.Success)
            return false;

        characterMessage = new CharacterMessage(incomingMessageMatch.Groups["characterName"].Value, incomingMessageMatch.Groups["message"].Value, MessageSource.Player, DateTime.Now);
        return true;
    }

    public bool TryParseOutgoingCharacterMessage(string line, out CharacterMessage? characterMessage)
    {
        characterMessage = null;
        var outgoingMessageMatch = outgoingMessagePattern.Match(line);
        if (!outgoingMessageMatch.Success)
            return false;

        characterMessage = new CharacterMessage(outgoingMessageMatch.Groups["characterName"].Value, outgoingMessageMatch.Groups["message"].Value, MessageSource.Me, DateTime.Now);
        return true;
    }

    public bool IsGeneralMessage(string line, out string character, out string message)
    {
        character = "";
        message = "";
        if(generalMessageNamePattern.IsMatch(line))
            character = generalMessageNamePattern.Match(line).ToString();
        else if (tradeMessageNamePattern.IsMatch(line))
            character = tradeMessageNamePattern.Match(line).ToString();
        else
            return false;
        
        if(guildNamePattern.IsMatch(character))
            character = guildNamePattern.Replace(character, "");
        
        message = messagePattern.Match(line).ToString();
        return true;
    }

    public bool IsFreeMaster(string line, out string master, out string character)
    {
        string[] backup = {"haku", "elreon", "catarina", "cat", "tora", "vorici", "vagan", "zana"};
        string[] masters = {};//"truth", "fear", "doubt", "grief", "rage", "pain", "zana"};

        master = "";
        string message;
        if(!IsGeneralMessage(line, out character, out message) || message.ToLower().Contains("wtb") || message.ToLower().Contains("lf "))
            return false;

        foreach (var m in masters)
        {
            if (!message.ToLower().Contains(m) || message.ToLower().Contains("full") || message.ToLower().Contains("lf")) continue;
            master = m;
            return true;
        }
        return false;
    }

    public string JoinedArea(string message)
    {
        var match = joinedAreaPattern.Match(message);
        return match.Success ? match.ToString() : "";
    }

    public string LeftArea(string message)
    {
        var match = leftAreaPattern.Match(message);
        return match.Success ? match.ToString() : "";
    }

    public bool ChangedArea(string message)
    {
        var match = changedAreaPattern.Match(message);
        return match.Success;
    }

    public bool FailedChangeArea(string message)
    {
        var match = failedChangeAreaPattern.Match(message);
        return match.Success;
    }
}
