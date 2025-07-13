using Google.Protobuf;
using PoeLib.JSON;
using PoeLib.Parsers;
using System;
using System.Linq;

namespace PoeLib.Proto;

public static class MessageConverters
{
    public static CharacterMessage FromProto(this PoeTradeMonitorProto.CharacterMessage message)
    {
        return new CharacterMessage
        {
            Character = message.Character,
            Message = message.Message,
            Timestamp = DateTime.FromBinary(message.Timestamp),
            Source = (MessageSource)message.Source
        };
    }

    public static Price FromProto(this PoeTradeMonitorProto.Price protoPrice)
    {
        var price = new Price();
        foreach (var currency in protoPrice.Currencies)
        {
            price.Currencies.Add(new Currency
            {
                Type = (CurrencyType)currency.Type,
                Amount = Convert.ToDecimal(currency.Amount)
            });
        }
        return price;
    }

    public static ItemTradeRequest FromProto(this PoeTradeMonitorProto.ItemTradeRequest request)
    {
        var item = request.Item;
        var requestItem = new Item
        {
            Name = item.Name,
            ItemLevel = item.ItemLevel,
            stackSize = item.StackSize,
            BaseType = item.BaseType,
            corrupted = item.Corrupted,
            shaper = item.Shaper,
            elder = item.Elder,
            veiled = item.Veiled,
            rarity = (Rarity)item.Rarity,
            descrText = item.DescrText ?? string.Empty,
            Sockets = item.Sockets.Select(sock => new Socket{ type = sock.Type, group = sock.Group, color = (SocketColor)sock.Color }).ToList(),
            rawImplicitMods = item.ImplicitMods.ToList(),
            rawExplicitMods = item.ExplicitMods.ToList()
        };

        return new ItemTradeRequest
        {
            CharacterName = request.CharacterName,
            AccountName = request.AccountName,
            Price = request.Price.FromProto(),
            Timestamp = DateTime.FromBinary(request.Timestamp),
            Item = requestItem,
            DivineRate = Convert.ToDecimal(request.DivineRate)
        };
    }

    public static ByteString ToByteString(this byte[] byteArray)
    {
        return ByteString.CopyFrom(byteArray);
    }

    public static PoeTradeMonitorProto.Socket ToProtoSocket(this Socket socket)
    {
        return new PoeTradeMonitorProto.Socket() { Group = socket.group, Type = socket.type, Color = (PoeTradeMonitorProto.Socket.Types.SocketColor)socket.color };
    }


    public static PoeTradeMonitorProto.Item ToProtoItem(this Item item)
    {
        var protoItem = new PoeTradeMonitorProto.Item()
        {
            Name = item.Name,
            ItemLevel = item.ItemLevel,
            StackSize = item.stackSize,
            BaseType = item.BaseType,
            Corrupted = item.corrupted,
            Shaper = item.shaper,
            Elder = item.elder,
            Veiled = item.veiled,
            Rarity = (PoeTradeMonitorProto.Item.Types.Rarity)item.rarity,
            DescrText = item.descrText ?? string.Empty
        };

        foreach (var socket in item.Sockets)
            protoItem.Sockets.Add(socket.ToProtoSocket());

        foreach (var mod in item.ImplicitMods)
            protoItem.ImplicitMods.Add(mod.RawModText);

        foreach (var mod in item.ExplicitMods)
            protoItem.ExplicitMods.Add(mod.RawModText);

        return protoItem;
    }

    public static PoeTradeMonitorProto.Price ToProtoPrice(this Price price)
    {
        var protoPrice = new PoeTradeMonitorProto.Price();
        foreach(var currency in price.Currencies)
        {
            protoPrice.Currencies.Add(new PoeTradeMonitorProto.Currency
            {
                Type = (int)currency.Type,
                Amount = Convert.ToDouble(currency.Amount)
            });
        }
        return protoPrice;
    }
}
