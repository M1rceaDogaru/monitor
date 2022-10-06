using Monitor.API;
using Monitor.Grains;
using Monitor.Shared;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Streams;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "cors_policy",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader();
                      });
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddSingleton<NotificationsObserver>();
builder.Services.AddSingleton(serviceProvider =>
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

    client.Connect().Wait();

    var stream = client
        .GetStreamProvider(Constants.NotificationsChannel)
        .GetStream<string>(Constants.NotificationsStreamId, Constants.NotificationsNamespace);

    stream.SubscribeAsync(serviceProvider.GetService<NotificationsObserver>()).Wait();

    return client;
});

var app = builder.Build();

app.UseCors("cors_policy");

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
