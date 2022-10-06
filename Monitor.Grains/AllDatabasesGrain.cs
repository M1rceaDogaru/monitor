using Monitor.Shared;
using Orleans;

namespace Monitor.Grains
{
    public interface IAllDatabasesGrain : IGrainWithIntegerKey
    {
        Task<List<DatabaseState>> GetAll();
        Task Upsert(DatabaseState state);
    }

    public class AllDatabasesGrain : Grain, IAllDatabasesGrain
    {
        private readonly List<DatabaseState> _allStates = new();

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
