using PoeLib.Common;
using PoeTrade.Contracts.Extensions;

namespace PoeLib.Extensions;

public static class MessageConverters
{
    public static CharacterMessage FromProto(this Proto.CharacterMessage message)
    {
        return new CharacterMessage(message.Character, message.Message, (MessageSource)message.Source, DateTime.FromBinary(message.Timestamp));
    }

    public static Proto.CharacterMessage ToProto(this CharacterMessage message)
    {
        return new Proto.CharacterMessage()
        {
            Character = message.Character,
            Message = message.Message,
            Timestamp = message.Timestamp.ToBinary(),
            Source = (Proto.CharacterMessage.Types.MessageSource)message.Source
        };
    }

    public static ItemTradeRequest FromProto(this Proto.ItemTradeRequest request)
    {
        return new ItemTradeRequest(request.CharacterName, request.AccountName, request.Item.ToRecord(), request.Price.ToRecord(), request.Currencies.Select(c => c.ToRecord()).ToList(), DateTime.FromBinary(request.Timestamp), Convert.ToDecimal(request.DivineRate));
    }

    public static Proto.ItemTradeRequest ToProto(this ItemTradeRequest request)
    {
        var itemTradeRequest = new Proto.ItemTradeRequest()
        {
            CharacterName = request.CharacterName,
            AccountName = request.AccountName,
            Item = request.Item.ToProtobuf(),
            Price = request.Price.ToProtobuf(),
            Timestamp = request.Timestamp.ToBinary(),
            DivineRate = Convert.ToDouble(request.DivineRate)
        };

        foreach (var currency in request.Currencies)
            itemTradeRequest.Currencies.Add(currency.ToProtobuf());

        return itemTradeRequest;
    }
}
