using System.CommandLine;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;

namespace InternetMonitor.Cli.Commands;

public sealed class ExportCommand : ICommandModule
{
    private readonly IExportService _exportService;

    public ExportCommand(IExportService exportService)
    {
        _exportService = exportService;
    }

    public Command Create()
    {
        var command = new Command("export", "Export internet monitor data.");

        var formetOption = new Option<ExportFormat>("--format")
        {
            Description = "Export format (json, csv, pdf)",
            DefaultValueFactory = _ => ExportFormat.Json
        };

        var limitOption = new Option<int>("--limit")
        {
            Description = "Number of records to export.",
            DefaultValueFactory = _ => 100
        };

        command.Options.Add(formetOption);
        command.Options.Add(limitOption);

        command.SetAction(async (parseResult, token) =>
        {
            var format = parseResult.GetValue(formetOption);
            var limit = parseResult.GetValue(limitOption);

            try
            {
                var result = await _exportService.ExportAsync(
                    format,
                    limit,
                    token);

                ConsoleOutput.PrintExport(result);
            }
            catch (Exception ex)
            {
                ConsoleOutput.PrintError($"Export failed: {ex.Message}");
            }
        });

        return command;
    }
}