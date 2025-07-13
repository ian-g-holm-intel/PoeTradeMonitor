using System.Threading.Tasks;
using Moq.AutoMock;
using NUnit.Framework;
using PoeLib.Tools.Notification;

namespace PoeLib.Tests;

[TestFixture]
public class PushoverNotificationTests
{
    private PushoverNotificationClient client;
    private AutoMocker mocker;

    [SetUp]
    public void Setup()
    {
        mocker = new AutoMocker();
        client = mocker.CreateInstance<PushoverNotificationClient>();
    }

    [Test]
    public async Task SendPushNotification()
    {
        await client.SendPushNotification("Test", "SubTest", "Body");
        await Task.Delay(5000);
    }
}
