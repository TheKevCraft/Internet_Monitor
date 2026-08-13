using InternetMonitor.Core.BackgroundServices;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInternetMonitorCore();

builder.Services.AddHostedService<InternetMonitorWorker>();

var host = builder.Build();

var initializer = host.Services.GetRequiredService<IDatabaseInitializer>();

await initializer.InitializeAsync();

await host.RunAsync();
