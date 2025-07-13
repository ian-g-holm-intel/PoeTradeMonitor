using System;
using System.Net;

namespace PoeLib;

public class InsufficientCurrencyException : Exception
{
    public InsufficientCurrencyException() : base("Tried to remove more currency than what is available") { }
}

public class FailedGetTabException : Exception
{
    public FailedGetTabException() : base("Failed to get tab, check tab naming or restart PoE") { }
}

public class CurrencyParseException : Exception
{
    public CurrencyParseException(string message, Exception ex = null) : base($"Failed to parse currency from message: {message}", ex) { }
}

public class CurrencyPriceNotFoundException : Exception
{
    public CurrencyPriceNotFoundException(CurrencyType type) : base($"No price data found for {type}") { }
}

public class TradeFailureException : Exception
{
    public TradeFailureException() : base("Player failed to complete the trade") { }
    protected TradeFailureException(string message) : base(message) { }
}

public class FreeItemSlotException : TradeFailureException
{
    public FreeItemSlotException() : base("Unable to find free item slot in inventory") { }
    protected FreeItemSlotException(string message) : base(message) { }
}

public class TradeClosedException : TradeFailureException
{
    public TradeClosedException() : base("Player closed trade window") { }
}

public class GameClosedException : TradeFailureException
{
    public GameClosedException() : base("The game is not running") { }
}

public class WrongItemException : TradeFailureException
{
    public WrongItemException(string itemInfo) : base($"Player put the wrong item in trade window: {itemInfo}") { }
}

public class AttemptedScamException : TradeFailureException
{
    public AttemptedScamException(string itemInfo) : base($"Player attempted to scam: {itemInfo}") { }
}

public class RemoveCurrencyFailedException : TradeFailureException
{
    public RemoveCurrencyFailedException() : base("Failed to remove currency from inventory") { }
}

public class TradeTimeoutException : TradeFailureException
{
    public TradeTimeoutException() : base("Timed out waiting for item to be placed in trade window") { }
}

public class FailedEnterHideoutException : TradeFailureException
{
    public FailedEnterHideoutException(string characterName) : base(string.IsNullOrEmpty(characterName) ? "Failed to enter hideout" : $"Failed to enter the hideout of {characterName}") { }
}

public class LiveSearchException : Exception
{
    public LiveSearchException() : base("Failed to start live search") { }
    public LiveSearchException(string error) : base($"Failed to start live search: {error}") { }
}

public class CurrencyExchangeException : Exception
{
    public CurrencyExchangeException() : base("Failed to get currency exchange data") { }
    public CurrencyExchangeException(string error) : base($"Failed to get currency exchange data: {error}") { }
    public CurrencyExchangeException(string error, Exception innerException) : base($"Failed to get currency exchange data: {error}", innerException) { }
    public CurrencyExchangeException(HttpStatusCode statusCode) : base($"Failed to get currency exchange data, Status: {statusCode}") { }
}

public class ItemWhisperException : Exception
{
    public ItemWhisperException(string message) : base(message) { }
}

public class MemoryReadException : Exception
{
    public MemoryReadException() : base("Failed reading an object from PoE memory") { }
    public MemoryReadException(string error) : base($"Failed reading an object from PoE memory: {error}") { }
}

public class StashOpenFailureException : Exception
{
    public StashOpenFailureException() : base("Failed to open stash, player position unknown") { }
}

public class SocialPanelNotInitializedException : Exception
{
    public SocialPanelNotInitializedException() : base("Social panel not initialized") { }
}

public class ProxyNotAvailableException : Exception
{
    public ProxyNotAvailableException() : base("There is not a proxy available") { }
}
