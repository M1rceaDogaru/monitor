namespace Monitor.Shared
{
    public static class Constants
    {
        public const string NotificationsChannel = "notifications";
        public const string NotificationsNamespace = "default";
        public static readonly Guid NotificationsStreamId = Guid.NewGuid();

        public const string Cluster = "dev";
        public const string Service = "Monitor";
        public const int AllDatabasesGrainKey = 1;
    }
}
