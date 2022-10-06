using Orleans.Streams;

namespace Monitor.API
{
    public class NotificationsObserver : IAsyncObserver<string>
    {
        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.FromException(ex);
        }

        public Task OnNextAsync(string item, StreamSequenceToken token = null)
        {
            // TODO: push to signalr

            return Task.CompletedTask;
        }
    }
}
