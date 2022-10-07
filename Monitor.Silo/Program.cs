using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Monitor.Shared;
using Orleans.Configuration;
using Orleans.Hosting;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            var host = await StartSiloAsync();
            Console.WriteLine("\n\n Press Enter to terminate...\n\n");
            Console.ReadLine();

            await host.StopAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 1;
        }

        static async Task<IHost> StartSiloAsync()
        {
            var builder = new HostBuilder()
                .UseOrleans(s =>
                {
                    s
                    .UseLocalhostClustering()
                    .AddMemoryGrainStorage("PubSubStore")
                    .AddSimpleMessageStreamProvider(Constants.NotificationsChannel, options =>
                    {
                        options.FireAndForgetDelivery = false;
                        options.OptimizeForImmutableData = false;
                    })
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = Constants.Cluster;
                        options.ServiceId = Constants.Service;
                    })
                    .ConfigureLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Warning));
                });

            var host = builder.Build();
            await host.StartAsync();

            return host;
        }
    }
}