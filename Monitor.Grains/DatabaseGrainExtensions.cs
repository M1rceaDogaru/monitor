using Monitor.Shared;

namespace Monitor.Grains
{
    public static class DatabaseGrainExtensions
    {
        public static string GetKey(this DatabaseState state)
        {
            return $"{state.Host}/{state.Instance}/{state.Database}";
        }
    }
}
