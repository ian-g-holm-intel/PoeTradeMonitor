using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PoeLib.Parsers;

public interface IMessageParser
{
    CurrencyTradeOffer ParseCurrencyTradeOffer(string message);
    ItemTradeOffer ParseItemTradeOffer(string message);
    bool IsOutOfLeague(string message);
    bool TryParseIncomingCharacterMessage(string line, out CharacterMessage characterMessage);
    bool TryParseOutgoingCharacterMessage(string line, out CharacterMessage characterMessage);
    bool IsGeneralMessage(string line, out string character, out string message);
    bool IsFreeMaster(string line, out string master, out string character);
    string JoinedArea(string message);
    string LeftArea(string message);
    bool ChangedArea(string message);
    bool FailedChangeArea(string message);
    bool IsIgnoredMessage(string message);
    bool TryGetAutoreply(string message, out string reply);
}

public class MessageParser : IMessageParser
{
    private readonly Regex incomingMessagePattern = new Regex(@"@From(?: <[^>]*>)? (?<characterName>[^:]+): (?<message>.+)$", RegexOptions.Compiled);
    private readonly Regex outgoingMessagePattern = new Regex(@"@To(?: <[^>]*>)? (?<characterName>[^:]+): (?<message>.+)$", RegexOptions.Compiled);
    private readonly Regex generalMessageNamePattern = new Regex(@"(?<=#)(<[^<>]+> )?[^ ]+(?=:)", RegexOptions.Compiled);
    private readonly Regex tradeMessageNamePattern = new Regex(@"(?<=\$)(<[^<>]+> )?[^ ]+(?=:)", RegexOptions.Compiled);
    private readonly Regex guildNamePattern = new Regex(@"<[^<>]+> ", RegexOptions.Compiled);
    private readonly Regex messagePattern = new Regex(@"(?<=: ).+", RegexOptions.Compiled);
    private readonly Regex myCurrencyPattern = new Regex(@"(?<=your )[\d]+ [\w' ]+(?= for)", RegexOptions.Compiled);
    private readonly Regex theirCurrencyPattern = new Regex(@"(?<=my )[\d]+ [\w' ]+(?= in)", RegexOptions.Compiled);
    private readonly Regex joinedAreaPattern = new Regex(@"[^ ]+(?= has joined the area)", RegexOptions.Compiled);
    private readonly Regex leftAreaPattern = new Regex(@"[^ ]+(?= has left the area)", RegexOptions.Compiled);
    private readonly Regex changedAreaPattern = new Regex(@"Generating level [\d]+ area", RegexOptions.Compiled);
    private readonly Regex failedChangeAreaPattern = new Regex(@"Failed to join", RegexOptions.Compiled);
    private readonly Regex outOfLeaguePattern = new Regex(@"That character is out of your league", RegexOptions.Compiled);
    private readonly Regex amountPattern = new Regex(@"\d+");
    private readonly string[] ignoredMessages = {"ready", "sorry", "thank you", "ty", "tx", "t4t", "thanks", "one sec", "one min", "thankyou", "thx", "gl hf", "glhf", "gl", "dnd", "autoreply", "afk.", "sold", "a sec", "1 sec", "a min"};
    private readonly Dictionary<string, string> autoreplyMessages = new Dictionary<string, string>{ { "how many", "just one" }, { "want both", "just one" }, {"how much", "just one" }, {"both?", "just one" }, {"all?", "just one" }, {"1?", "ya"}, { "one?", "ya" }, { "2", "just one" }, { "3", "just one" }, { "4", "just one" }, { "5", "just one" }, { "6", "just one" }, { "7", "just one" }, { "8", "just one" }, { "9", "just one" }, { "still interested?", "no thanks" }, {"still need", "no thanks"} };

    public CurrencyTradeOffer ParseCurrencyTradeOffer(string message)
    {
        var myCurrencyMatch = myCurrencyPattern.Match(message);
        var theirCurrencyMatch = theirCurrencyPattern.Match(message);
        if (!myCurrencyMatch.Success || !theirCurrencyMatch.Success) return null;

        var characterNameMatch = incomingMessagePattern.Match(message);
        if (!characterNameMatch.Success)
            throw new CurrencyParseException(message);

        try
        {
            var characterName = guildNamePattern.Replace(characterNameMatch.ToString(), "");

            var myCurrencyAmountString = amountPattern.Match(myCurrencyMatch.ToString()).ToString();
            var myCurrencyTypeString = myCurrencyMatch.ToString().Replace(myCurrencyAmountString, "").Replace("'", "").Trim();
            var myCurrencyAmount = int.Parse(myCurrencyAmountString);
            var myCurrencyType = myCurrencyTypeString.GetCurrencyType();

            var theirCurrencyAmountString = amountPattern.Match(theirCurrencyMatch.ToString()).ToString();
            var theirCurrencyTypeString = theirCurrencyMatch.ToString().Replace(theirCurrencyAmountString, "").Replace("'", "").Trim();
            var theirCurrencyAmount = int.Parse(theirCurrencyAmountString);
            var theirCurrencyType = theirCurrencyTypeString.GetCurrencyType();

            if (myCurrencyType == CurrencyType.none || theirCurrencyType == CurrencyType.none)
                return null;

            return new CurrencyTradeOffer{Timestamp = DateTime.Now, CharacterName = characterName, Mine = new Currency{Amount = myCurrencyAmount, Type = myCurrencyType}, Theirs = new Currency {Amount = theirCurrencyAmount, Type = theirCurrencyType} };
        }
        catch (Exception ex)
        {
            throw new CurrencyParseException(message, ex);
        }
    }

    public ItemTradeOffer ParseItemTradeOffer(string message)
    {
        return null;
    }

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

    public bool IsIgnoredMessage(string message)
    {
        if (message.Contains("Hi, I would like to buy")) return false;
        var lowerCaseMessage = message.ToLower();
        return ignoredMessages.Any(i => lowerCaseMessage.Equals(i));
    }

    public bool TryParseIncomingCharacterMessage(string line, out CharacterMessage characterMessage)
    {
        characterMessage = new CharacterMessage();
        var incomingMessageMatch = incomingMessagePattern.Match(line);
        if (!incomingMessageMatch.Success)
            return false;

        characterMessage.Timestamp = DateTime.Now;
        characterMessage.Source = MessageSource.Player;
        characterMessage.Character = incomingMessageMatch.Groups["characterName"].Value;
        characterMessage.Message = incomingMessageMatch.Groups["message"].Value;
        return true;
    }

    public bool TryParseOutgoingCharacterMessage(string line, out CharacterMessage characterMessage)
    {
        characterMessage = new CharacterMessage();
        var outgoingMessageMatch = outgoingMessagePattern.Match(line);
        if (!outgoingMessageMatch.Success)
            return false;

        characterMessage.Timestamp = DateTime.Now;
        characterMessage.Source = MessageSource.Me;
        characterMessage.Character = outgoingMessageMatch.Groups["characterName"].Value;
        characterMessage.Message = outgoingMessageMatch.Groups["message"].Value;
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
