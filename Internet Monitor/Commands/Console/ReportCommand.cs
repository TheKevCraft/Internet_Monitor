using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Internet_Monitor.Commands.Console;

public class ReportCommand : IConsoleCommand
{
    public string Name => "report";
    public string Description => "";
    public string Usage => "";
    public IEnumerable<string> Aliases => [];

    public async Task ExecuteAsync(CommandContext context)
    {
        var report = context.Services.GetRequiredService<IReportService>();

        var result = await report.GenerateDailyReportAsync(24);

        AnsiConsole.Write(
            new Panel(result)
            {
                Header = new PanelHeader("Daily Report"),
                Border = BoxBorder.Rounded
            });
    }
}
