using Microsoft.Extensions.Logging;
using PushoverClient;
using System;
using System.Threading.Tasks;

namespace PoeLib.Tools.Notification;

public class PushoverNotificationClient : INotificationClient
{
    private readonly ILogger<PushoverNotificationClient> logger;
    private Pushover client;
    private Options options;

    public PushoverNotificationClient(ILogger<PushoverNotificationClient> logger)
    {
        client = new Pushover(Environment.GetEnvironmentVariable("PUSHOVER_API_KEY"));
        options = new Options
        {
            Recipients = Environment.GetEnvironmentVariable("PUSHOVER_USER_TOKEN"), //User, group or comma separated values
            Priority = Priority.Normal,
            Notification = NotificationSound.PhoneDefault,
            Html = true
        };
        this.logger = logger;
    }

    public void RegisterDeviceToken(string token)
    {
    }

    public void ResetBadgeCount()
    {
    }

    public async Task SendPushNotification(string title, string subtitle, string body, string sound = "keys.caf")
    {
        try
        {
            var response = await client.PushAsync(title, $"{subtitle} - {body}", options);
            while (response.Status != 1)
            {
                response = await client.PushAsync(title, $"{subtitle} - {body}", options);
                await Task.Delay(500);
            }
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to send push notification");
        }
    }
}
