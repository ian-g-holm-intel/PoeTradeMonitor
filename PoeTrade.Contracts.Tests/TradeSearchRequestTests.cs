using PoeTrade.Contracts;

namespace PoeTrade.Contracts.Tests;

[TestClass]
public class TradeSearchRequestTests
{
    [TestMethod]
    public void Name_WithQueryName_ShouldReturnQueryName()
    {
        var request = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = "Test Item Name" }
        };

        Assert.AreEqual("Test Item Name", request.Name);
    }

    [TestMethod]
    public void Name_WithoutQueryNameButWithType_ShouldReturnQueryType()
    {
        var request = new TradeSearchRequest
        {
            Query = new TradeQuery { Type = "Weapon" }
        };

        Assert.AreEqual("Weapon", request.Name);
    }

    [TestMethod]
    public void Name_WithEmptyQueryNameButWithType_ShouldReturnQueryType()
    {
        var request = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = "", Type = "Armor" }
        };

        Assert.AreEqual("Armor", request.Name);
    }

    [TestMethod]
    public void Name_WithoutQueryNameAndType_ShouldReturnUnknown()
    {
        var request = new TradeSearchRequest
        {
            Query = new TradeQuery()
        };

        Assert.AreEqual("Unknown", request.Name);
    }

    [TestMethod]
    public void Name_WithNullQueryNameAndType_ShouldReturnUnknown()
    {
        var request = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = null, Type = null }
        };

        Assert.AreEqual("Unknown", request.Name);
    }

    [TestMethod]
    public void Name_WithQueryNamePrecedenceOverType_ShouldReturnQueryName()
    {
        var request = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = "Specific Item", Type = "Generic Type" }
        };

        Assert.AreEqual("Specific Item", request.Name);
    }

    [TestMethod]
    public void ToString_ShouldReturnNameProperty()
    {
        var request = new TradeSearchRequest
        {
            Query = new TradeQuery { Name = "Test Item" }
        };

        Assert.AreEqual("Test Item", request.ToString());
    }

    [TestMethod]
    public void ToString_WithTypeOnly_ShouldReturnType()
    {
        var request = new TradeSearchRequest
        {
            Query = new TradeQuery { Type = "Weapon Type" }
        };

        Assert.AreEqual("Weapon Type", request.ToString());
    }

    [TestMethod]
    public void ToString_WithNoNameOrType_ShouldReturnUnknown()
    {
        var request = new TradeSearchRequest
        {
            Query = new TradeQuery()
        };

        Assert.AreEqual("Unknown", request.ToString());
    }
}