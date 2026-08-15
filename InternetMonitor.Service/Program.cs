using InternetMonitor.Core.BackgroundServices;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Extensions;
using InternetMonitor.Core.Constants;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = MonitorServiceConstants.ServiceName;
});

builder.Services.AddInternetMonitorCore();

builder.Services.AddHostedService<InternetMonitorWorker>();

var host = builder.Build();

var initializer = host.Services.GetRequiredService<IDatabaseInitializer>();

await initializer.InitializeAsync();

await host.RunAsync();
