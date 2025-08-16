using Moq.AutoMock;
using PoeLib.Tools.Notification;

namespace PoeLib.Tests;

[TestClass]
public class PushoverNotificationTests
{
    private PushoverNotificationClient client = null!;
    private AutoMocker mocker = null!;

    [TestInitialize]
    public void Setup()
    {
        mocker = new AutoMocker();
        client = mocker.CreateInstance<PushoverNotificationClient>();
    }

    [TestMethod]
    [Ignore]
    public async Task SendPushNotification()
    {
        await client.SendPushNotification("Test", "SubTest", "Body");
    }
}
