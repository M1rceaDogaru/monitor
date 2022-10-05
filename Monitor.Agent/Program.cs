using Monitor.Agent;
using System.CommandLine;
class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("Starting agent...");
        var mothershipOption = new Option<string>(
            name: "--mothership",
            description: "The URL of the parent service where changes are notified");

        var connectionOption = new Option<string>(
            name: "--connection",
            description: "Connection string of the database to monitor");

        var rootCommand = new RootCommand("Sample app for System.CommandLine");
        rootCommand.AddOption(connectionOption);
        rootCommand.AddOption(mothershipOption);

        rootCommand.SetHandler((connection, mothership) =>
        {
            var databaseTracking = new DatabaseTracking(connection,mothership);
            databaseTracking.Track();
        }, connectionOption, mothershipOption);

        return await rootCommand.InvokeAsync(args);
    }
}