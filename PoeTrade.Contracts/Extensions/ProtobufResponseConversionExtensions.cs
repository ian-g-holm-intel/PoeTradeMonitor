namespace PoeTrade.Contracts.Extensions;

/// <summary>
/// Simplified extension methods to convert between C# records and protobuf classes
/// </summary>
public static class ProtobufResponseConversionExtensions
{
    #region TradeFetchResponse Conversions

    /// <summary>
    /// Converts a TradeFetchResponse record to protobuf message
    /// </summary>
    public static Proto.TradeFetchResponse ToProtobuf(this TradeFetchResponse record)
    {
        var proto = new Proto.TradeFetchResponse();
        proto.Result.AddRange(record.Result.Select(r => r.ToProtobuf()));
        return proto;
    }

    /// <summary>
    /// Converts a protobuf TradeFetchResponse to record
    /// </summary>
    public static TradeFetchResponse ToRecord(this Proto.TradeFetchResponse proto)
    {
        return new TradeFetchResponse
        {
            Result = proto.Result.Select(r => r.ToRecord()).ToList()
        };
    }

    #endregion

    #region TradeSearchResult Conversions

    /// <summary>
    /// Converts a TradeSearchResult record to protobuf message
    /// </summary>
    public static Proto.TradeSearchResult ToProtobuf(this TradeSearchResult record)
    {
        return new Proto.TradeSearchResult
        {
            Id = record.Id,
            Listing = record.Listing.ToProtobuf(),
            Item = record.Item.ToProtobuf()
        };
    }

    /// <summary>
    /// Converts a protobuf TradeSearchResult to record
    /// </summary>
    public static TradeSearchResult ToRecord(this Proto.TradeSearchResult proto)
    {
        return new TradeSearchResult
        {
            Id = proto.Id,
            Listing = proto.Listing.ToRecord(),
            Item = proto.Item.ToRecord()
        };
    }

    #endregion

    #region TradeListing Conversions

    /// <summary>
    /// Converts a TradeListing record to protobuf message
    /// </summary>
    public static Proto.TradeListing ToProtobuf(this TradeListing record)
    {
        var proto = new Proto.TradeListing
        {
            Method = record.Method,
            Indexed = record.Indexed.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Stash = record.Stash.ToProtobuf(),
            Whisper = record.Whisper,
            WhisperToken = record.WhisperToken,
            Account = record.Account.ToProtobuf()
        };

        // Handle nullable price properties
        if (record.Price != null)
        {
            if (!string.IsNullOrEmpty(record.Price.PriceType))
                proto.PriceType = record.Price.PriceType;
            if (record.Price.Amount > 0)
                proto.PriceAmount = (double)record.Price.Amount;
            var priceCurrencyString = TradeCurrencyTypeConverter.GetStringValue(record.Price.CurrencyType);
            if (!string.IsNullOrEmpty(priceCurrencyString))
                proto.PriceCurrency = priceCurrencyString;
        }

        return proto;
    }

    /// <summary>
    /// Converts a protobuf TradeListing to record
    /// </summary>
    public static TradeListing ToRecord(this Proto.TradeListing proto)
    {
        PriceInfo? price = null;
        if (proto.PriceType != null || proto.PriceAmount != null || proto.PriceCurrency != null)
        {
            var currencyType = TradeCurrencyTypeConverter.GetEnumValue(proto.PriceCurrency ?? string.Empty) ?? default(TradeCurrencyType);
            price = new PriceInfo
            {
                PriceType = proto.PriceType ?? string.Empty,
                Amount = (decimal)(proto.PriceAmount ?? 0),
                CurrencyType = currencyType
            };
        }

        return new TradeListing
        {
            Method = proto.Method,
            Indexed = DateTime.Parse(proto.Indexed).ToUniversalTime(),
            Stash = proto.Stash.ToRecord(),
            Whisper = proto.Whisper,
            WhisperToken = proto.WhisperToken,
            Account = proto.Account.ToRecord(),
            Price = price
        };
    }

    #endregion

    #region StashInfo Conversions

    /// <summary>
    /// Converts a StashInfo record to protobuf message
    /// </summary>
    public static Proto.StashInfo ToProtobuf(this StashInfo record)
    {
        return new Proto.StashInfo
        {
            Name = record.Name,
            X = record.X,
            Y = record.Y
        };
    }

    /// <summary>
    /// Converts a protobuf StashInfo to record
    /// </summary>
    public static StashInfo ToRecord(this Proto.StashInfo proto)
    {
        return new StashInfo
        {
            Name = proto.Name,
            X = proto.X,
            Y = proto.Y
        };
    }

    #endregion

    #region AccountInfo Conversions

    /// <summary>
    /// Converts an AccountInfo record to protobuf message
    /// </summary>
    public static Proto.AccountInfo ToProtobuf(this AccountInfo record)
    {
        var result = new Proto.AccountInfo
        {
            Name = record.Name,
            LastCharacterName = record.LastCharacterName,
            Language = record.Language,
            Realm = record.Realm
        };

        if (record.Online != null)
        {
            result.Online = record.Online.ToProtobuf();
        }

        return result;
    }

    /// <summary>
    /// Converts a protobuf AccountInfo to record
    /// </summary>
    public static AccountInfo ToRecord(this Proto.AccountInfo proto)
    {
        return new AccountInfo
        {
            Name = proto.Name,
            Online = proto.Online.ToRecord(),
            LastCharacterName = proto.LastCharacterName,
            Language = proto.Language,
            Realm = proto.Realm
        };
    }

    #endregion

    #region OnlineStatus Conversions

    /// <summary>
    /// Converts an OnlineStatus record to protobuf message
    /// </summary>
    public static Proto.OnlineStatus ToProtobuf(this OnlineStatus record)
    {
        var proto = new Proto.OnlineStatus
        {
            League = record.League
        };

        if (!string.IsNullOrEmpty(record.Status))
            proto.Status = record.Status;

        return proto;
    }

    /// <summary>
    /// Converts a protobuf OnlineStatus to record
    /// </summary>
    public static OnlineStatus ToRecord(this Proto.OnlineStatus proto)
    {
        return new OnlineStatus
        {
            League = proto.League,
            Status = proto.Status
        };
    }

    #endregion

    #region Basic TradeItem Conversions

    /// <summary>
    /// Converts a TradeItem record to protobuf message (basic properties only)
    /// </summary>
    public static Proto.TradeItem ToProtobuf(this TradeItem record)
    {
        var proto = new Proto.TradeItem
        {
            Verified = record.Verified,
            Width = record.Width,
            Height = record.Height,
            Icon = record.Icon,
            League = record.League,
            Id = record.Id,
            Name = record.Name,
            TypeLine = record.TypeLine,
            BaseType = record.BaseType,
            Rarity = record.Rarity.ToString(),
            ItemLevel = record.ItemLevel,
            Identified = record.Identified,
            FrameType = record.FrameType
        };

        // Handle nullable wrapper properties
        if (!string.IsNullOrEmpty(record.Realm))
            proto.Realm = record.Realm;
        if (record.Support.HasValue)
            proto.Support = record.Support.Value;
        proto.StackSize = record.StackSize;
        if (record.MaxStackSize.HasValue)
            proto.MaxStackSize = record.MaxStackSize.Value;
        if (!string.IsNullOrEmpty(record.Note))
            proto.Note = record.Note;
        if (record.Corrupted.HasValue)
            proto.Corrupted = record.Corrupted.Value;
        if (record.Socket.HasValue)
            proto.Socket = record.Socket.Value;

        // Handle collections
        if (record.Sockets != null)
            proto.Sockets.AddRange(record.Sockets.Select(s => s.ToProtobuf()));
        if (record.EnchantMods != null)
            proto.EnchantMods.AddRange(record.EnchantMods);
        if (record.ImplicitMods != null)
            proto.ImplicitMods.AddRange(record.ImplicitMods.Select(m => m.RawModText));
        if (record.ExplicitMods != null)
            proto.ExplicitMods.AddRange(record.ExplicitMods.Select(m => m.RawModText));
        if (record.FracturedMods != null)
            proto.FracturedMods.AddRange(record.FracturedMods.Select(m => m.RawModText));
        if (record.FlavourText != null)
            proto.FlavourText.AddRange(record.FlavourText);

        return proto;
    }

    /// <summary>
    /// Converts a protobuf TradeItem to record (basic properties only)
    /// </summary>
    public static TradeItem ToRecord(this Proto.TradeItem proto)
    {
        return new TradeItem
        {
            Realm = proto.Realm,
            Verified = proto.Verified,
            Width = proto.Width,
            Height = proto.Height,
            Icon = proto.Icon,
            Support = proto.Support,
            StackSize = proto.StackSize ?? 1,
            MaxStackSize = proto.MaxStackSize,
            League = proto.League,
            Id = proto.Id,
            Sockets = proto.Sockets.Select(s => s.ToRecord()).ToList(),
            Name = proto.Name,
            TypeLine = proto.TypeLine,
            BaseType = proto.BaseType,
            ItemLevel = proto.ItemLevel,
            Identified = proto.Identified,
            Note = proto.Note,
            Corrupted = proto.Corrupted,
            EnchantMods = proto.EnchantMods.ToList(),
            ImplicitMods = ConvertStringArrayToMods(proto.ImplicitMods),
            ExplicitMods = ConvertStringArrayToMods(proto.ExplicitMods),
            FracturedMods = ConvertStringArrayToMods(proto.FracturedMods),
            FlavourText = proto.FlavourText.ToList(),
            FrameType = proto.FrameType,
            Socket = proto.Socket
        };
    }

    #endregion

    #region SocketInfo Conversions

    /// <summary>
    /// Converts a SocketInfo record to protobuf message
    /// </summary>
    public static Proto.SocketInfo ToProtobuf(this SocketInfo record)
    {
        var proto = new Proto.SocketInfo
        {
            Group = record.Group
        };

        if (!string.IsNullOrEmpty(record.Attr))
            proto.Attr = record.Attr;
        if (!string.IsNullOrEmpty(record.SColour))
            proto.SColour = record.SColour;
        if (!string.IsNullOrEmpty(record.Type))
            proto.Type = record.Type;
        if (!string.IsNullOrEmpty(record.Item))
            proto.Item = record.Item;

        return proto;
    }

    /// <summary>
    /// Converts a protobuf SocketInfo to record
    /// </summary>
    public static SocketInfo ToRecord(this Proto.SocketInfo proto)
    {
        return new SocketInfo
        {
            Group = proto.Group,
            Attr = proto.Attr,
            SColour = proto.SColour,
            Type = proto.Type,
            Item = proto.Item
        };
    }

    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Converts a collection of mod strings to a list of Mod objects using the ModListConverter logic.
    /// </summary>
    private static List<Mod>? ConvertStringArrayToMods(IEnumerable<string> modStrings)
    {
        if (!modStrings.Any())
        {
            return null;
        }
        
        return ModListConverter.ParseModStrings(modStrings);
    }
    
    #endregion
}