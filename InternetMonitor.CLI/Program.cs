using InternetMonitor.Cli.Commands;
using InternetMonitor.CLI;
using InternetMonitor.Core.Extensions;
using InternetMonitor.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInternetMonitorCore();
builder.Services.AddInternetMonitorCli();

builder.Services.AddTransient<SpeedTestCommand>();
builder.Services.AddTransient<ConnectivityCommand>();
builder.Services.AddTransient<MonitorCommand>();

using var host = builder.Build();

var initializer = host.Services.GetRequiredService<IDatabaseInitializer>();

await initializer.InitializeAsync();

var rootCommand = new RootCommand("InternetMonitor CLI");

CommandLoader.RegisterCommands(rootCommand, host.Services);

return await rootCommand.Parse(args).InvokeAsync();