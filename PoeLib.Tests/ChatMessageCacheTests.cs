using Microsoft.Extensions.Logging;
using Moq;
using PoeLib.Common;
using PoeLib.Tools;

namespace PoeLib.Tests;

[TestClass]
public class ChatMessageCacheTests
{
    private Mock<ILogger<ChatMessageCache>> mockLogger = null!;
    private ChatMessageCache cache = null!;

    [TestInitialize]
    public void Setup()
    {
        mockLogger = new Mock<ILogger<ChatMessageCache>>();
        cache = new ChatMessageCache(mockLogger.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        cache?.Dispose();
    }

    [TestMethod]
    public void GetMessages_WhenEmpty_ShouldReturnEmptyArray()
    {
        var result = cache.GetMessages();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void AddMessage_ShouldAddMessageToCache()
    {
        var message = new CharacterMessage("TestPlayer", "Hello World", MessageSource.Player, DateTime.Now);

        cache.AddMessage(message);

        var result = cache.GetMessages();
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("TestPlayer", result[0].Character);
        Assert.AreEqual("Hello World", result[0].Message);
    }

    [TestMethod]
    public void AddMessage_WithNullMessage_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => cache.AddMessage(null!));
    }

    [TestMethod]
    public void GetMessages_ShouldReturnMessagesOrderedByTimestamp()
    {
        var oldMessage = new CharacterMessage("Player1", "First", MessageSource.Player, DateTime.Now.AddMinutes(-1));
        var newMessage = new CharacterMessage("Player2", "Second", MessageSource.Player, DateTime.Now);

        cache.AddMessage(newMessage);
        cache.AddMessage(oldMessage);

        var result = cache.GetMessages();
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("First", result[0].Message);
        Assert.AreEqual("Second", result[1].Message);
    }

    [TestMethod]
    public void ClearMessages_ShouldRemoveAllMessages()
    {
        var message1 = new CharacterMessage("Player1", "Test1", MessageSource.Player, DateTime.Now);
        var message2 = new CharacterMessage("Player2", "Test2", MessageSource.Player, DateTime.Now);

        cache.AddMessage(message1);
        cache.AddMessage(message2);
        cache.ClearMessages();

        var result = cache.GetMessages();
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void AddMessage_WhenExceedingThreshold_ShouldTriggerCleanup()
    {
        // Add messages beyond the cleanup threshold (1200)
        for (int i = 0; i < 1250; i++)
        {
            var message = new CharacterMessage($"Player{i}", $"Message{i}", MessageSource.Player, DateTime.Now.AddMinutes(i));
            cache.AddMessage(message);
        }

        var result = cache.GetMessages();
        // Cleanup is triggered when exceeding 1200, but final count might be slightly over 1000
        // since cleanup doesn't happen on every add. We just verify it's less than the threshold
        Assert.IsTrue(result.Length < 1200, $"Expected < 1200 messages after cleanup, but got {result.Length}");
        Assert.IsTrue(result.Length >= 1000, $"Expected at least 1000 messages to remain, but got {result.Length}");
    }

    [TestMethod]
    public void Dispose_ShouldClearAllMessages()
    {
        var message = new CharacterMessage("Player1", "Test", MessageSource.Player, DateTime.Now);
        cache.AddMessage(message);

        cache.Dispose();

        var result = cache.GetMessages();
        Assert.AreEqual(0, result.Length);
    }

    [TestMethod]
    public void AddMessage_MultipleMessages_ShouldMaintainOrder()
    {
        var messages = new List<CharacterMessage>();
        for (int i = 0; i < 10; i++)
        {
            var message = new CharacterMessage($"Player{i}", $"Message{i}", MessageSource.Player, DateTime.Now.AddMinutes(i));
            messages.Add(message);
            cache.AddMessage(message);
        }

        var result = cache.GetMessages();
        Assert.AreEqual(10, result.Length);
        
        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual($"Message{i}", result[i].Message);
        }
    }

    [TestMethod]
    public void AddMessage_WithDifferentMessageSources_ShouldStoreCorrectly()
    {
        var playerMessage = new CharacterMessage("Player1", "Player message", MessageSource.Player, DateTime.Now);
        var myMessage = new CharacterMessage("Me", "My message", MessageSource.Me, DateTime.Now.AddMinutes(1));

        cache.AddMessage(playerMessage);
        cache.AddMessage(myMessage);

        var result = cache.GetMessages();
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual(MessageSource.Player, result[0].Source);
        Assert.AreEqual(MessageSource.Me, result[1].Source);
    }
}