namespace Monitor.Shared
{
    public static class Constants
    {
        public const string NotificationsChannel = "notifications";
        public const string NotificationsNamespace = "default";
        public static readonly Guid NotificationsStreamId = new("d988a404-da13-4e3b-95b6-8533c1035661");

        public const string Cluster = "dev";
        public const string Service = "Monitor";
        public const int AllDatabasesGrainKey = 1;
    }
}
