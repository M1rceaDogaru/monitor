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
builder.Services.AddSingleton(clientBuilder =>
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
    client.Connect();

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

app.Run();
