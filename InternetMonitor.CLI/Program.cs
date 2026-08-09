using InternetMonitor.Cli.Commands;
using System.CommandLine;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using InternetMonitor.Core.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInternetMonitorCore();

builder.Services.AddTransient<SpeedTestCommand>();
builder.Services.AddTransient<ConnectivityCommand>();

using var host = builder.Build();

var initializer = host.Services.GetRequiredService<IDatabaseInitializer>();

await initializer.InitializeAsync();

var rootCommand = new RootCommand("InternetMonitor CLI");

CommandLoader.RegisterCommands(rootCommand, host.Services);

return await rootCommand.Parse(args).InvokeAsync();