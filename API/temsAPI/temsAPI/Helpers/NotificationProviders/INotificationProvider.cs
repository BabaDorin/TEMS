using System.Threading.Tasks;

namespace temsAPI.Helpers.NotificationProviders
{
    interface INotificationProvider
    {
        Task<string> SendNotification();
    }
}
