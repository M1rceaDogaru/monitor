using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.AspNetCore.SignalR.Client;
using Monitor.Shared;

namespace Monitor.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		var baseUrl = "http://localhost:5257";

		var connection = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}/{Constants.NotificationsChannel}")
                .Build();

        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
        };

		connection.On<string>(Constants.NotificationsChannel, message =>
		{
            var duration = ToastDuration.Short;
            var fontSize = 14;
            var toast = Toast.Make(message, duration, fontSize);

            toast.Show();
        });

		connection.StartAsync();

        return builder.Build();
	}
}
