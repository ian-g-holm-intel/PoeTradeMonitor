using System.Collections.Concurrent;
using System.Linq;

namespace PoeLib.Tools;

public interface IChatMessageCache
{
    CharacterMessage[] GetMessages();
    void AddMessage(CharacterMessage message);
    void ClearMessages();
}

public class ChatMessageCache : IChatMessageCache
{
    private ConcurrentBag<CharacterMessage> characterMessages = new ConcurrentBag<CharacterMessage>();

    public CharacterMessage[] GetMessages()
    {
        return characterMessages.OrderBy(item => item.Timestamp).ToArray();
    }

    public void AddMessage(CharacterMessage message)
    {
        characterMessages.Add(message);
    }

    public void ClearMessages()
    {
        characterMessages = new ConcurrentBag<CharacterMessage>();
    }
}
