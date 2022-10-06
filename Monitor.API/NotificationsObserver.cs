using Microsoft.AspNetCore.SignalR;
using Monitor.Shared;
using Orleans.Streams;

namespace Monitor.API
{
    public class NotificationsObserver : IAsyncObserver<string>
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public NotificationsObserver(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.FromException(ex);
        }

        public async Task OnNextAsync(string item, StreamSequenceToken token = null)
        {
            await _hubContext.Clients.All.SendAsync(Constants.NotificationsChannel, item);
        }
    }
}
