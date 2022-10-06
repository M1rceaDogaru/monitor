using Microsoft.AspNetCore.SignalR;
using Monitor.Shared;

namespace Monitor.API
{
    public class NotificationsHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync(Constants.NotificationsChannel, message);
        }
    }
}
