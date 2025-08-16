using PoeLib.Common;
using PoeLib.Extensions;

namespace PoeLib.Tests;

[TestClass]
public class MessageConvertersTests
{
    [TestMethod]
    public void FromProto_CharacterMessage_ShouldConvertCorrectly()
    {
        var timestamp = DateTime.Now;
        var protoMessage = new Proto.CharacterMessage
        {
            Character = "TestPlayer",
            Message = "Hello World",
            Source = Proto.CharacterMessage.Types.MessageSource.Player,
            Timestamp = timestamp.ToBinary()
        };

        var result = protoMessage.FromProto();

        Assert.AreEqual("TestPlayer", result.Character);
        Assert.AreEqual("Hello World", result.Message);
        Assert.AreEqual(MessageSource.Player, result.Source);
        Assert.AreEqual(timestamp, result.Timestamp);
    }

    [TestMethod]
    public void ToProto_CharacterMessage_ShouldConvertCorrectly()
    {
        var timestamp = DateTime.Now;
        var message = new CharacterMessage("TestPlayer", "Hello World", MessageSource.Me, timestamp);

        var result = message.ToProto();

        Assert.AreEqual("TestPlayer", result.Character);
        Assert.AreEqual("Hello World", result.Message);
        Assert.AreEqual(Proto.CharacterMessage.Types.MessageSource.Me, result.Source);
        Assert.AreEqual(timestamp.ToBinary(), result.Timestamp);
    }

    [TestMethod]
    public void CharacterMessage_RoundTripConversion_ShouldPreserveData()
    {
        var originalMessage = new CharacterMessage("Player1", "Test message", MessageSource.Player, DateTime.Now);

        var protoMessage = originalMessage.ToProto();
        var convertedBack = protoMessage.FromProto();

        Assert.AreEqual(originalMessage.Character, convertedBack.Character);
        Assert.AreEqual(originalMessage.Message, convertedBack.Message);
        Assert.AreEqual(originalMessage.Source, convertedBack.Source);
        Assert.AreEqual(originalMessage.Timestamp, convertedBack.Timestamp);
    }

    [TestMethod]
    public void FromProto_CharacterMessage_WithDifferentMessageSources_ShouldConvertCorrectly()
    {
        var playerMessage = new Proto.CharacterMessage
        {
            Character = "Player1",
            Message = "Player message",
            Source = Proto.CharacterMessage.Types.MessageSource.Player,
            Timestamp = DateTime.Now.ToBinary()
        };

        var meMessage = new Proto.CharacterMessage
        {
            Character = "Me",
            Message = "My message",
            Source = Proto.CharacterMessage.Types.MessageSource.Me,
            Timestamp = DateTime.Now.ToBinary()
        };

        var playerResult = playerMessage.FromProto();
        var meResult = meMessage.FromProto();

        Assert.AreEqual(MessageSource.Player, playerResult.Source);
        Assert.AreEqual(MessageSource.Me, meResult.Source);
    }
}