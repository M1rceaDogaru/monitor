using Monitor.API;
using Monitor.Grains;
using Monitor.Shared;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddSingleton(async serviceProvider =>
{
    var client = new ClientBuilder()
        .UseLocalhostClustering()
        .Configure<ClusterOptions>(options =>
        {
            options.ClusterId = Constants.Cluster;
            options.ServiceId = Constants.Service;
        })
        .ConfigureApplicationParts(
            parts => parts.AddApplicationPart(typeof(IDatabaseGrain).Assembly).WithReferences())
        .AddSimpleMessageStreamProvider(Constants.NotificationsChannel)
        .ConfigureLogging(logging => logging.AddConsole())
        .Build();

    await client.Connect();

    var stream = client
        .GetStreamProvider(Constants.NotificationsChannel)
        .GetStream<string>(Constants.NotificationsStreamId, Constants.NotificationsNamespace);

    await stream.SubscribeAsync(new NotificationsObserver());

    return client;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationsHub>($"/{Constants.NotificationsChannel}");

app.Run();
