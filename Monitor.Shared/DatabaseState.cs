namespace Monitor.Shared
{
    public class DatabaseState
    {
        public string Host { get; set; }
        public string Instance { get; set; }
        public string Database { get; set; }

        public List<string> Users { get; set; } = new List<string>();

        public override bool Equals(object? obj)
        {
            if (obj is DatabaseState state)
            {
                return state.Host == Host && state.Instance == Instance && state.Database == Database &&
                    state.Users.Count == Users.Count && state.Users.SequenceEqual(Users);
            }

            return false;
        }
    }
}