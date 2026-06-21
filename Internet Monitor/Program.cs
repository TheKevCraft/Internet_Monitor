using Internet_Monitor.BackgroundServices;
using Internet_Monitor.Interfaces;
using Internet_Monitor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddDebug();

builder.Services.AddHttpClient<IConnectivityService, ConnectivityService>();
builder.Services.AddSingleton<ISpeedTestService, SpeedTestService>();
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSingleton<IExportService, ExportService>();

builder.Services.AddSingleton<ConsoleUiService>();
builder.Services.AddHostedService<SchedulerService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetService<IDataService>();

    if (dataService != null)
        await dataService.InitializeAsync();
}

using var cts = new CancellationTokenSource();

var ui = host.Services.GetRequiredService<ConsoleUiService>();

var uiTask = ui.RunAsync(cts.Token);

await host.RunAsync(cts.Token);
await uiTask;