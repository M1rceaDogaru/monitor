using Microsoft.Extensions.Logging;
using Monitor.Shared;
using Orleans;
using Orleans.Placement;
using Orleans.Streams;

namespace Monitor.Grains
{
    public interface IDatabaseGrain : IGrainWithStringKey
    {
        Task UpdateState(DatabaseState state);
    }

    [RandomPlacement]
    public class DatabaseGrain : Grain, IDatabaseGrain
    {
        private DatabaseState _state;
        private IAsyncStream<string> _stream = null!;
        private IAllDatabasesGrain _allDatabasesGrain = null!;

        private readonly ILogger<DatabaseGrain> _logger;

        public DatabaseGrain(ILogger<DatabaseGrain> logger)
        {
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(Constants.NotificationsChannel);

            _stream = streamProvider.GetStream<string>(
                Constants.NotificationsStreamId, Constants.NotificationsNamespace);

            _allDatabasesGrain = GrainFactory.GetGrain<IAllDatabasesGrain>(Constants.AllDatabasesGrainKey);

            _logger.LogWarning($"Grain activated for {this.GetPrimaryKeyString()}");

            return base.OnActivateAsync();
        }

        public async Task UpdateState(DatabaseState state)
        {
            var messages = new Messages();
            if (_state == null)
            {
                SetState(state);
                return;
            }

            if (!state.Equals(_state))
            {
                // something changed
                messages.AddRange(CheckRules(_state, state));
                SetState(state);
            }

            _logger.LogWarning($"State updated for {_state.Key}");
            foreach (var message in messages)
            {
                await _stream.OnNextAsync(message);
            }
        }

        private static Messages CheckRules(DatabaseState oldState, DatabaseState newState)
        {
            // Just a hardcoded rule checking for changes in users

            var messages = new Messages();
            var usersAdded = newState.Users.Except(oldState.Users);
            var usersRemoved = oldState.Users.Except(newState.Users);

            if (usersAdded.Any())
            {
                messages.Add($"Users {string.Join(",", usersAdded)} added to {newState.Key}");
            }

            if (usersRemoved.Any())
            {
                messages.Add($"Users {string.Join(",", usersRemoved)} removed from {newState.Key}");
            }

            return messages;
        }

        private void SetState(DatabaseState state)
        {
            _state = state;
            _allDatabasesGrain.Upsert(state);
        }
    }
}
