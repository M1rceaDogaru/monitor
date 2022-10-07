using Microsoft.Extensions.Logging;
using Monitor.Shared;
using Orleans;
using Orleans.Placement;

namespace Monitor.Grains
{
    public interface IAllDatabasesGrain : IGrainWithIntegerKey
    {
        Task<List<DatabaseState>> GetAll();
        Task Upsert(DatabaseState state);
    }

    [RandomPlacement]
    public class AllDatabasesGrain : Grain, IAllDatabasesGrain
    {
        private readonly List<DatabaseState> _allStates = new();
        private readonly ILogger<DatabaseGrain> _logger;

        public AllDatabasesGrain(ILogger<DatabaseGrain> logger)
        {
            _logger = logger;
        }

        public override Task OnActivateAsync()
        {
            _logger.LogWarning("Databases store grain activated");
            return base.OnActivateAsync();
        }

        public Task Upsert(DatabaseState state)
        {
            var existingState = _allStates.FirstOrDefault(s => s.Key == state.Key);
            if (existingState != null)
            {
                _allStates.Remove(existingState);
            }

            _allStates.Add(state);
            return Task.CompletedTask;
        }

        public Task<List<DatabaseState>> GetAll()
        {
            return Task.FromResult(_allStates.ToList());
        }
    }
}
