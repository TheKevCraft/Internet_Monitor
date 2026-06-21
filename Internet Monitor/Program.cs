using Internet_Monitor.BackgroundServices;
using Internet_Monitor.Interfaces;
using Internet_Monitor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<IConnectivityService, ConnectivityService>();
builder.Services.AddSingleton<ISpeedTestService, SpeedTestService>();
builder.Services.AddSingleton<IDataService, DataService>();

builder.Services.AddHostedService<SchedulerService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetService<IDataService>();

    if (dataService != null)
        await dataService.InitializeAsync();
}

await host.RunAsync();