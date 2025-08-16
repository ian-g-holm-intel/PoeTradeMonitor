using PoeLib.Common;
using PoeTrade.Contracts;
using System.Net;

namespace PoeLib.Tests;

[TestClass]
public class ExceptionsTests
{
    [TestMethod]
    public void InsufficientCurrencyException_ShouldHaveCorrectMessage()
    {
        var exception = new InsufficientCurrencyException();

        Assert.AreEqual("Tried to remove more currency than what is available", exception.Message);
    }

    [TestMethod]
    public void FailedGetTabException_ShouldHaveCorrectMessage()
    {
        var exception = new FailedGetTabException();

        Assert.AreEqual("Failed to get tab, check tab naming or restart PoE", exception.Message);
    }

    [TestMethod]
    public void CurrencyParseException_WithMessage_ShouldFormatCorrectly()
    {
        var testMessage = "invalid currency string";
        var exception = new CurrencyParseException(testMessage);

        Assert.AreEqual($"Failed to parse currency from message: {testMessage}", exception.Message);
    }

    [TestMethod]
    public void CurrencyParseException_WithMessageAndInnerException_ShouldPreserveBoth()
    {
        var testMessage = "invalid currency string";
        var innerException = new ArgumentException("Inner exception");
        var exception = new CurrencyParseException(testMessage, innerException);

        Assert.AreEqual($"Failed to parse currency from message: {testMessage}", exception.Message);
        Assert.AreEqual(innerException, exception.InnerException);
    }

    [TestMethod]
    public void CurrencyPriceNotFoundException_ShouldFormatWithCurrencyType()
    {
        var currencyType = TradeCurrencyType.Divine;
        var exception = new CurrencyPriceNotFoundException(currencyType);

        Assert.AreEqual($"No price data found for {currencyType}", exception.Message);
    }

    [TestMethod]
    public void TradeFailureException_ShouldHaveCorrectMessage()
    {
        var exception = new TradeFailureException();

        Assert.AreEqual("Player failed to complete the trade", exception.Message);
    }

    [TestMethod]
    public void FreeItemSlotException_ShouldHaveCorrectMessage()
    {
        var exception = new FreeItemSlotException();

        Assert.AreEqual("Unable to find free item slot in inventory", exception.Message);
    }

    [TestMethod]
    public void TradeClosedException_ShouldHaveCorrectMessage()
    {
        var exception = new TradeClosedException();

        Assert.AreEqual("Player closed trade window", exception.Message);
    }

    [TestMethod]
    public void GameClosedException_ShouldHaveCorrectMessage()
    {
        var exception = new GameClosedException();

        Assert.AreEqual("The game is not running", exception.Message);
    }

    [TestMethod]
    public void WrongItemException_ShouldFormatWithItemInfo()
    {
        var itemInfo = "Unique Sword instead of Chaos Orb";
        var exception = new WrongItemException(itemInfo);

        Assert.AreEqual($"Player put the wrong item in trade window: {itemInfo}", exception.Message);
    }

    [TestMethod]
    public void AttemptedScamException_ShouldFormatWithItemInfo()
    {
        var itemInfo = "6-link instead of 5-link";
        var exception = new AttemptedScamException(itemInfo);

        Assert.AreEqual($"Player attempted to scam: {itemInfo}", exception.Message);
    }

    [TestMethod]
    public void RemoveCurrencyFailedException_ShouldHaveCorrectMessage()
    {
        var exception = new RemoveCurrencyFailedException();

        Assert.AreEqual("Failed to remove currency from inventory", exception.Message);
    }

    [TestMethod]
    public void TradeTimeoutException_ShouldHaveCorrectMessage()
    {
        var exception = new TradeTimeoutException();

        Assert.AreEqual("Timed out waiting for item to be placed in trade window", exception.Message);
    }

    [TestMethod]
    public void FailedEnterHideoutException_WithCharacterName_ShouldFormatWithName()
    {
        var characterName = "TestPlayer";
        var exception = new FailedEnterHideoutException(characterName);

        Assert.AreEqual($"Failed to enter the hideout of {characterName}", exception.Message);
    }

    [TestMethod]
    public void FailedEnterHideoutException_WithEmptyCharacterName_ShouldUseGenericMessage()
    {
        var exception = new FailedEnterHideoutException(string.Empty);

        Assert.AreEqual("Failed to enter hideout", exception.Message);
    }

    [TestMethod]
    public void FailedEnterHideoutException_WithNullCharacterName_ShouldUseGenericMessage()
    {
        var exception = new FailedEnterHideoutException(null!);

        Assert.AreEqual("Failed to enter hideout", exception.Message);
    }

    [TestMethod]
    public void LiveSearchException_DefaultConstructor_ShouldHaveCorrectMessage()
    {
        var exception = new LiveSearchException();

        Assert.AreEqual("Failed to start live search", exception.Message);
    }

    [TestMethod]
    public void LiveSearchException_WithError_ShouldFormatWithError()
    {
        var error = "Connection timeout";
        var exception = new LiveSearchException(error);

        Assert.AreEqual($"Failed to start live search: {error}", exception.Message);
    }

    [TestMethod]
    public void CurrencyExchangeException_DefaultConstructor_ShouldHaveCorrectMessage()
    {
        var exception = new CurrencyExchangeException();

        Assert.AreEqual("Failed to get currency exchange data", exception.Message);
    }

    [TestMethod]
    public void CurrencyExchangeException_WithError_ShouldFormatWithError()
    {
        var error = "API rate limit exceeded";
        var exception = new CurrencyExchangeException(error);

        Assert.AreEqual($"Failed to get currency exchange data: {error}", exception.Message);
    }

    [TestMethod]
    public void CurrencyExchangeException_WithErrorAndInnerException_ShouldPreserveBoth()
    {
        var error = "Network error";
        var innerException = new HttpRequestException("Connection failed");
        var exception = new CurrencyExchangeException(error, innerException);

        Assert.AreEqual($"Failed to get currency exchange data: {error}", exception.Message);
        Assert.AreEqual(innerException, exception.InnerException);
    }

    [TestMethod]
    public void CurrencyExchangeException_WithStatusCode_ShouldFormatWithStatus()
    {
        var statusCode = HttpStatusCode.NotFound;
        var exception = new CurrencyExchangeException(statusCode);

        Assert.AreEqual($"Failed to get currency exchange data, Status: {statusCode}", exception.Message);
    }

    [TestMethod]
    public void ItemWhisperException_ShouldPreserveMessage()
    {
        var message = "Failed to send whisper message";
        var exception = new ItemWhisperException(message);

        Assert.AreEqual(message, exception.Message);
    }

    [TestMethod]
    public void MemoryReadException_DefaultConstructor_ShouldHaveCorrectMessage()
    {
        var exception = new MemoryReadException();

        Assert.AreEqual("Failed reading an object from PoE memory", exception.Message);
    }

    [TestMethod]
    public void MemoryReadException_WithError_ShouldFormatWithError()
    {
        var error = "Access violation";
        var exception = new MemoryReadException(error);

        Assert.AreEqual($"Failed reading an object from PoE memory: {error}", exception.Message);
    }

    [TestMethod]
    public void StashOpenFailureException_ShouldHaveCorrectMessage()
    {
        var exception = new StashOpenFailureException();

        Assert.AreEqual("Failed to open stash, player position unknown", exception.Message);
    }

    [TestMethod]
    public void SocialPanelNotInitializedException_ShouldHaveCorrectMessage()
    {
        var exception = new SocialPanelNotInitializedException();

        Assert.AreEqual("Social panel not initialized", exception.Message);
    }

    [TestMethod]
    public void TradeFailureExceptions_ShouldInheritFromTradeFailureException()
    {
        Assert.IsTrue(typeof(FreeItemSlotException).IsSubclassOf(typeof(TradeFailureException)));
        Assert.IsTrue(typeof(TradeClosedException).IsSubclassOf(typeof(TradeFailureException)));
        Assert.IsTrue(typeof(GameClosedException).IsSubclassOf(typeof(TradeFailureException)));
        Assert.IsTrue(typeof(WrongItemException).IsSubclassOf(typeof(TradeFailureException)));
        Assert.IsTrue(typeof(AttemptedScamException).IsSubclassOf(typeof(TradeFailureException)));
        Assert.IsTrue(typeof(RemoveCurrencyFailedException).IsSubclassOf(typeof(TradeFailureException)));
        Assert.IsTrue(typeof(TradeTimeoutException).IsSubclassOf(typeof(TradeFailureException)));
        Assert.IsTrue(typeof(FailedEnterHideoutException).IsSubclassOf(typeof(TradeFailureException)));
    }
}