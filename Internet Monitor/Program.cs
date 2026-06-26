using Internet_Monitor.BackgroundServices;
using Internet_Monitor.Commands.Console;
using Internet_Monitor.Interfaces;
using Internet_Monitor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

bool isHeadless = args.Contains("--headless");

QuestPDF.Settings.License = LicenseType.Community;

builder.Logging.ClearProviders();
builder.Logging.AddDebug();

builder.Services.AddHttpClient<IConnectivityService, ConnectivityService>();
builder.Services.AddSingleton<ISpeedTestService, SpeedTestService>();
builder.Services.AddSingleton<IDataService, DataService>();
builder.Services.AddSingleton<IExportService, ExportService>();
builder.Services.AddSingleton<IExecutionPolicyService, ExecutionPolicyService>();
builder.Services.AddSingleton<IReportService, ReportService>();
builder.Services.AddSingleton<IPdfReportExporter, PdfReportExporter>();

// Commands
builder.Services.AddSingleton<IConsoleCommand, HelpCommand>();
builder.Services.AddSingleton<IConsoleCommand, StatusCommand>();
builder.Services.AddSingleton<IConsoleCommand, HistoryCommand>();
builder.Services.AddSingleton<IConsoleCommand, ExportCommand>();
builder.Services.AddSingleton<IConsoleCommand, ReportCommand>();
builder.Services.AddSingleton<IConsoleCommand, SpeedTestCommand>();
/*builder.Services.AddSingleton<IConsoleCommand, ExitCommand>();
/*builder.Services.AddSingleton<IConsoleCommand, ClearCommand>();*/

builder.Services.AddHostedService<SchedulerService>();

if (!isHeadless)
{
    builder.Services.AddSingleton<ConsoleUiService>();
}

var host = builder.Build();

// DB Init
using (var scope = host.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetService<IDataService>();

    if (dataService != null)
        await dataService.InitializeAsync();
}

// UI nur im Dev Mode
if (!isHeadless)
{
    var ui = host.Services.GetRequiredService<ConsoleUiService>();

    _ = ui.RunAsync(CancellationToken.None);
}
await host.RunAsync();