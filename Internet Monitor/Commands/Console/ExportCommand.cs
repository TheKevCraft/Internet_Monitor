using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using static Internet_Monitor.Services.Helper;

namespace Internet_Monitor.Commands.Console;

public class ExportCommand : IConsoleCommand
{
    public string Name => "export";
    public string Description => "";
    public string Usage => "";
    public IEnumerable<string> Aliases => [];

    public async Task ExecuteAsync(CommandContext context)
    {
        var exportService = context.Services.GetRequiredService<IExportService>();
        var pdfExporter = context.Services.GetRequiredService<IPdfReportExporter>();
        var reportService = context.Services.GetRequiredService<IReportService>();

        if (context.Arguments.Length < 2)
        {
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine("export <csv|json|PDF> <speed|connectivity> [limit]");
            return;
        }

        var format = context.Arguments[0].ToLower();
        var type = context.Arguments[1].ToLower();
        var limit = GetLimit(context.Arguments, 2, 100);

        string path;

        if (format == "pdf" && type == "report")
        {
            var report = await reportService.GenerateReportAsync(limit);
            path = await pdfExporter.ExportAsync(report);
        }
        else if (type == "speed")
        {
            var fmt = format == "json" ? ExportFormat.Json : ExportFormat.Csv;
            path = await exportService.ExportSpeedTestsAsync(fmt, limit);
        }
        else if (type == "connectivity")
        {
            var fmt = format == "json" ? ExportFormat.Json : ExportFormat.Csv;
            path = await exportService.ExportConnectivityAsync(fmt, limit);
        }
        else
        {
            System.Console.WriteLine("Invalid export command.");
            return;
        }

        AnsiConsole.MarkupLine($"[green]Export:[/] {path}");
    }
}
