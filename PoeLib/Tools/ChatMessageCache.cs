using PoeLib.Common;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace PoeLib.Tools;

/// <summary>
/// Interface for caching chat messages with automatic cleanup.
/// </summary>
public interface IChatMessageCache
{
    CharacterMessage[] GetMessages();
    void AddMessage(CharacterMessage message);
    void ClearMessages();
}

/// <summary>
/// Thread-safe cache for chat messages with automatic expiration and size limits.
/// </summary>
public class ChatMessageCache : IChatMessageCache, IDisposable
{
    private readonly ConcurrentQueue<CharacterMessage> _messages = new();
    private readonly ILogger<ChatMessageCache> _logger;
    private readonly object _lockObject = new();
    private const int MaxMessages = 1000;
    private const int CleanupThreshold = 1200;
    
    public ChatMessageCache(ILogger<ChatMessageCache> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets all cached messages ordered by timestamp.
    /// </summary>
    public CharacterMessage[] GetMessages()
    {
        return _messages.OrderBy(item => item.Timestamp).ToArray();
    }

    /// <summary>
    /// Adds a message to the cache and performs cleanup if needed.
    /// </summary>
    public void AddMessage(CharacterMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        
        _messages.Enqueue(message);
        
        // Cleanup old messages if we exceed the threshold
        if (_messages.Count > CleanupThreshold)
        {
            CleanupOldMessages();
        }
    }

    /// <summary>
    /// Clears all cached messages.
    /// </summary>
    public void ClearMessages()
    {
        lock (_lockObject)
        {
            while (_messages.TryDequeue(out _)) { }
            _logger.LogDebug("Chat message cache cleared");
        }
    }
    
    /// <summary>
    /// Removes old messages to keep cache size manageable.
    /// </summary>
    private void CleanupOldMessages()
    {
        lock (_lockObject)
        {
            var removed = 0;
            while (_messages.Count > MaxMessages && _messages.TryDequeue(out _))
            {
                removed++;
            }
            
            if (removed > 0)
            {
                _logger.LogDebug("Removed {RemovedCount} old messages from cache", removed);
            }
        }
    }
    
    /// <summary>
    /// Disposes of the cache resources.
    /// </summary>
    public void Dispose()
    {
        ClearMessages();
        GC.SuppressFinalize(this);
    }
}
