using PoeLib.Common;
using PoeLib.Parsers;

namespace PoeLib.Tests;

[TestClass]
public class MessageParserTests
{
    private MessageParser parser = null!;

    [TestInitialize]
    public void Setup()
    {
        parser = new MessageParser();
    }

    [TestMethod]
    public void IsOutOfLeague_WithValidMessage_ShouldReturnTrue()
    {
        var message = "That character is out of your league";

        var result = parser.IsOutOfLeague(message);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsOutOfLeague_WithInvalidMessage_ShouldReturnFalse()
    {
        var message = "Some other message";

        var result = parser.IsOutOfLeague(message);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void TryParseIncomingCharacterMessage_WithValidMessage_ShouldReturnTrue()
    {
        var line = "@From TestPlayer: Hello there!";

        var result = parser.TryParseIncomingCharacterMessage(line, out var characterMessage);

        Assert.IsTrue(result);
        Assert.IsNotNull(characterMessage);
        Assert.AreEqual("TestPlayer", characterMessage.Character);
        Assert.AreEqual("Hello there!", characterMessage.Message);
        Assert.AreEqual(MessageSource.Player, characterMessage.Source);
    }

    [TestMethod]
    public void TryParseIncomingCharacterMessage_WithGuildTag_ShouldReturnTrue()
    {
        var line = "@From <GUILD> TestPlayer: Hello there!";

        var result = parser.TryParseIncomingCharacterMessage(line, out var characterMessage);

        Assert.IsTrue(result);
        Assert.IsNotNull(characterMessage);
        Assert.AreEqual("TestPlayer", characterMessage.Character);
        Assert.AreEqual("Hello there!", characterMessage.Message);
    }

    [TestMethod]
    public void TryParseIncomingCharacterMessage_WithInvalidFormat_ShouldReturnFalse()
    {
        var line = "Some random text";

        var result = parser.TryParseIncomingCharacterMessage(line, out var characterMessage);

        Assert.IsFalse(result);
        Assert.IsNull(characterMessage);
    }

    [TestMethod]
    public void TryParseOutgoingCharacterMessage_WithValidMessage_ShouldReturnTrue()
    {
        var line = "@To TestPlayer: Hi back!";

        var result = parser.TryParseOutgoingCharacterMessage(line, out var characterMessage);

        Assert.IsTrue(result);
        Assert.IsNotNull(characterMessage);
        Assert.AreEqual("TestPlayer", characterMessage.Character);
        Assert.AreEqual("Hi back!", characterMessage.Message);
        Assert.AreEqual(MessageSource.Me, characterMessage.Source);
    }

    [TestMethod]
    public void TryParseOutgoingCharacterMessage_WithGuildTag_ShouldReturnTrue()
    {
        var line = "@To <GUILD> TestPlayer: Hi back!";

        var result = parser.TryParseOutgoingCharacterMessage(line, out var characterMessage);

        Assert.IsTrue(result);
        Assert.IsNotNull(characterMessage);
        Assert.AreEqual("TestPlayer", characterMessage.Character);
        Assert.AreEqual("Hi back!", characterMessage.Message);
    }

    [TestMethod]
    public void IsIgnoredMessage_WithTradeMessage_ShouldReturnFalse()
    {
        var message = "Hi, I would like to buy your item";

        var result = parser.IsIgnoredMessage(message);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsIgnoredMessage_WithCommonIgnoredWord_ShouldReturnTrue()
    {
        var message = "thank you";

        var result = parser.IsIgnoredMessage(message);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsIgnoredMessage_WithDiscordLink_ShouldReturnTrue()
    {
        var message = "Join our discord server";

        var result = parser.IsIgnoredMessage(message);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsIgnoredMessage_WithAfkMessage_ShouldReturnTrue()
    {
        var message = "I'm afk right now";

        var result = parser.IsIgnoredMessage(message);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TryGetAutoreply_WithKnownQuestion_ShouldReturnTrue()
    {
        var message = "how many do you have?";

        var result = parser.TryGetAutoreply(message, out var reply);

        Assert.IsTrue(result);
        Assert.AreEqual("just one", reply);
    }

    [TestMethod]
    public void TryGetAutoreply_WithOneQuestion_ShouldReturnYa()
    {
        var message = "one?";

        var result = parser.TryGetAutoreply(message, out var reply);

        Assert.IsTrue(result);
        Assert.AreEqual("ya", reply);
    }

    [TestMethod]
    public void TryGetAutoreply_WithUnknownQuestion_ShouldReturnFalse()
    {
        var message = "some random question";

        var result = parser.TryGetAutoreply(message, out var reply);

        Assert.IsFalse(result);
        Assert.AreEqual("", reply);
    }

    [TestMethod]
    public void IsGeneralMessage_WithHashPrefix_ShouldReturnTrue()
    {
        var line = "#<GUILD> TestPlayer: Hello everyone!";

        var result = parser.IsGeneralMessage(line, out var character, out var message);

        Assert.IsTrue(result);
        Assert.AreEqual("TestPlayer", character);
        Assert.AreEqual("Hello everyone!", message);
    }

    [TestMethod]
    public void IsGeneralMessage_WithTradePrefix_ShouldReturnTrue()
    {
        var line = "$<GUILD> TestPlayer: WTS my item";

        var result = parser.IsGeneralMessage(line, out var character, out var message);

        Assert.IsTrue(result);
        Assert.AreEqual("TestPlayer", character);
        Assert.AreEqual("WTS my item", message);
    }

    [TestMethod]
    public void IsGeneralMessage_WithInvalidFormat_ShouldReturnFalse()
    {
        var line = "Some random text without prefixes";

        var result = parser.IsGeneralMessage(line, out var character, out var message);

        Assert.IsFalse(result);
        Assert.AreEqual("", character);
        Assert.AreEqual("", message);
    }

    [TestMethod]
    public void JoinedArea_WithValidMessage_ShouldReturnCharacterName()
    {
        var message = "TestPlayer has joined the area";

        var result = parser.JoinedArea(message);

        Assert.AreEqual("TestPlayer", result);
    }

    [TestMethod]
    public void JoinedArea_WithInvalidMessage_ShouldReturnEmpty()
    {
        var message = "Some other message";

        var result = parser.JoinedArea(message);

        Assert.AreEqual("", result);
    }

    [TestMethod]
    public void LeftArea_WithValidMessage_ShouldReturnCharacterName()
    {
        var message = "TestPlayer has left the area";

        var result = parser.LeftArea(message);

        Assert.AreEqual("TestPlayer", result);
    }

    [TestMethod]
    public void ChangedArea_WithValidMessage_ShouldReturnTrue()
    {
        var message = "Generating level 68 area";

        var result = parser.ChangedArea(message);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ChangedArea_WithInvalidMessage_ShouldReturnFalse()
    {
        var message = "Some other message";

        var result = parser.ChangedArea(message);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void FailedChangeArea_WithValidMessage_ShouldReturnTrue()
    {
        var message = "Failed to join the instance";

        var result = parser.FailedChangeArea(message);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void FailedChangeArea_WithInvalidMessage_ShouldReturnFalse()
    {
        var message = "Successfully joined";

        var result = parser.FailedChangeArea(message);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsFreeMaster_WithMasterMessage_ShouldReturnFalse()
    {
        // Since the masters array is empty in the current implementation,
        // this should return false
        var line = "#TestPlayer: Free zana mission";

        var result = parser.IsFreeMaster(line, out var master, out var character);

        Assert.IsFalse(result);
        Assert.AreEqual("", master);
    }

    [TestMethod]
    public void IsFreeMaster_WithWtbMessage_ShouldReturnFalse()
    {
        var line = "#TestPlayer: wtb some item";

        var result = parser.IsFreeMaster(line, out var master, out var character);

        Assert.IsFalse(result);
    }
}