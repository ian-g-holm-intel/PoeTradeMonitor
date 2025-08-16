using PoeTrade.Contracts.Extensions;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class ResponseProtobufConversionTests : BaseDeserializationTests
{
    [TestMethod]
    public async Task PoE1TradeItem_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Weapon.json");
        var originalItem = response.Result.First().Item;

        // Act - Convert to protobuf and back
        var protoItem = originalItem.ToProtobuf();
        var convertedItem = protoItem.ToRecord();

        // Assert - Basic properties should match
        Assert.AreEqual(originalItem.Id, convertedItem.Id);
        Assert.AreEqual(originalItem.Name, convertedItem.Name);
        Assert.AreEqual(originalItem.TypeLine, convertedItem.TypeLine);
        Assert.AreEqual(originalItem.Rarity, convertedItem.Rarity);
        Assert.AreEqual(originalItem.ItemLevel, convertedItem.ItemLevel);
        Assert.AreEqual(originalItem.Verified, convertedItem.Verified);
        Assert.AreEqual(originalItem.Width, convertedItem.Width);
        Assert.AreEqual(originalItem.Height, convertedItem.Height);
        Assert.AreEqual(originalItem.Icon, convertedItem.Icon);
        Assert.AreEqual(originalItem.League, convertedItem.League);
        Assert.AreEqual(originalItem.BaseType, convertedItem.BaseType);
        Assert.AreEqual(originalItem.Identified, convertedItem.Identified);
        Assert.AreEqual(originalItem.FrameType, convertedItem.FrameType);

        // Nullable properties
        Assert.AreEqual(originalItem.Realm, convertedItem.Realm);
        Assert.AreEqual(originalItem.Support, convertedItem.Support);
        Assert.AreEqual(originalItem.StackSize, convertedItem.StackSize);
        Assert.AreEqual(originalItem.MaxStackSize, convertedItem.MaxStackSize);
        Assert.AreEqual(originalItem.Note, convertedItem.Note);
        Assert.AreEqual(originalItem.Corrupted, convertedItem.Corrupted);
        Assert.AreEqual(originalItem.Socket, convertedItem.Socket);

        // Collections (handle null cases)
        if (originalItem.EnchantMods == null)
            Assert.IsTrue(convertedItem.EnchantMods == null || convertedItem.EnchantMods.Count == 0);
        else
            CollectionAssert.AreEqual(originalItem.EnchantMods, convertedItem.EnchantMods);
            
        if (originalItem.ImplicitMods == null)
            Assert.IsTrue(convertedItem.ImplicitMods == null || convertedItem.ImplicitMods.Count == 0);
        else
        {
            Assert.IsNotNull(convertedItem.ImplicitMods);
            Assert.AreEqual(originalItem.ImplicitMods.Count, convertedItem.ImplicitMods.Count);
            for (int i = 0; i < originalItem.ImplicitMods.Count; i++)
            {
                Assert.AreEqual(originalItem.ImplicitMods[i].RawModText, convertedItem.ImplicitMods[i].RawModText);
            }
        }
            
        if (originalItem.ExplicitMods == null)
            Assert.IsTrue(convertedItem.ExplicitMods == null || convertedItem.ExplicitMods.Count == 0);
        else
        {
            Assert.IsNotNull(convertedItem.ExplicitMods);
            Assert.AreEqual(originalItem.ExplicitMods.Count, convertedItem.ExplicitMods.Count);
            for (int i = 0; i < originalItem.ExplicitMods.Count; i++)
            {
                Assert.AreEqual(originalItem.ExplicitMods[i].RawModText, convertedItem.ExplicitMods[i].RawModText);
            }
        }
            
        if (originalItem.FlavourText == null)
            Assert.IsTrue(convertedItem.FlavourText == null || convertedItem.FlavourText.Count == 0);
        else
            CollectionAssert.AreEqual(originalItem.FlavourText, convertedItem.FlavourText);

        // Sockets
        Assert.AreEqual(originalItem.Sockets?.Count, convertedItem.Sockets?.Count);
        if (originalItem.Sockets != null && convertedItem.Sockets != null)
        {
            for (int i = 0; i < originalItem.Sockets.Count; i++)
            {
                Assert.AreEqual(originalItem.Sockets[i].Group, convertedItem.Sockets[i].Group);
                Assert.AreEqual(originalItem.Sockets[i].Attr, convertedItem.Sockets[i].Attr);
                Assert.AreEqual(originalItem.Sockets[i].SColour, convertedItem.Sockets[i].SColour);
                Assert.AreEqual(originalItem.Sockets[i].Type, convertedItem.Sockets[i].Type);
                Assert.AreEqual(originalItem.Sockets[i].Item, convertedItem.Sockets[i].Item);
            }
        }
    }

    [TestMethod]
    public async Task PoE2TradeItem_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var response = await DeserializeJsonFileAsync("Responses/PoE2/Weapon.json");
        var originalItem = response.Result.First().Item;

        // Act - Convert to protobuf and back
        var protoItem = originalItem.ToProtobuf();
        var convertedItem = protoItem.ToRecord();

        // Assert - Basic properties should match
        Assert.AreEqual(originalItem.Id, convertedItem.Id);
        Assert.AreEqual(originalItem.Name, convertedItem.Name);
        Assert.AreEqual(originalItem.TypeLine, convertedItem.TypeLine);
        Assert.AreEqual(originalItem.Rarity, convertedItem.Rarity);
        Assert.AreEqual(originalItem.ItemLevel, convertedItem.ItemLevel);
        Assert.AreEqual(originalItem.Realm, convertedItem.Realm);

        // PoE2 specific properties
        Assert.AreEqual("poe2", convertedItem.Realm);
    }

    [TestMethod]
    public async Task TradeListing_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var response = await DeserializeJsonFileAsync("Responses/PoE1/Rarity.json");
        var originalListing = response.Result.First().Listing;

        // Act - Convert to protobuf and back
        var protoListing = originalListing.ToProtobuf();
        var convertedListing = protoListing.ToRecord();

        // Assert
        Assert.AreEqual(originalListing.Method, convertedListing.Method);
        Assert.AreEqual(originalListing.Indexed, convertedListing.Indexed);
        Assert.AreEqual(originalListing.Whisper, convertedListing.Whisper);
        Assert.AreEqual(originalListing.WhisperToken, convertedListing.WhisperToken);

        // Stash info
        Assert.AreEqual(originalListing.Stash.Name, convertedListing.Stash.Name);
        Assert.AreEqual(originalListing.Stash.X, convertedListing.Stash.X);
        Assert.AreEqual(originalListing.Stash.Y, convertedListing.Stash.Y);

        // Account info
        Assert.AreEqual(originalListing.Account.Name, convertedListing.Account.Name);
        Assert.AreEqual(originalListing.Account.LastCharacterName, convertedListing.Account.LastCharacterName);
        Assert.AreEqual(originalListing.Account.Language, convertedListing.Account.Language);
        Assert.AreEqual(originalListing.Account.Realm, convertedListing.Account.Realm);
        Assert.AreEqual(originalListing.Account.Online?.League, convertedListing.Account.Online?.League);
        Assert.AreEqual(originalListing.Account.Online?.Status, convertedListing.Account.Online?.Status);

        // Price info
        if (originalListing.Price != null)
        {
            Assert.IsNotNull(convertedListing.Price);
            Assert.AreEqual(originalListing.Price.PriceType, convertedListing.Price.PriceType);
            Assert.AreEqual(originalListing.Price.CurrencyType, convertedListing.Price.CurrencyType);
            // Allow small decimal precision differences in conversion
            Assert.AreEqual((double)originalListing.Price.Amount, (double)convertedListing.Price.Amount, 0.01);
        }
    }

    [TestMethod]
    public async Task TradeSearchResponse_ShouldConvertToProtobufAndBack()
    {
        // Arrange
        var originalResponse = await DeserializeJsonFileAsync("Responses/PoE1/Rarity.json");

        // Act - Convert to protobuf and back
        var protoResponse = originalResponse.ToProtobuf();
        var convertedResponse = protoResponse.ToRecord();

        // Assert
        Assert.AreEqual(originalResponse.Result.Count, convertedResponse.Result.Count);
        
        for (int i = 0; i < originalResponse.Result.Count; i++)
        {
            var originalResult = originalResponse.Result[i];
            var convertedResult = convertedResponse.Result[i];
            
            Assert.AreEqual(originalResult.Id, convertedResult.Id);
            Assert.AreEqual(originalResult.Item.Id, convertedResult.Item.Id);
            Assert.AreEqual(originalResult.Item.Name, convertedResult.Item.Name);
            Assert.AreEqual(originalResult.Listing.Account.Name, convertedResult.Listing.Account.Name);
        }
    }
}