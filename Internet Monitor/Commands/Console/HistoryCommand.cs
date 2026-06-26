using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using static Internet_Monitor.Services.Helper;

namespace Internet_Monitor.Commands.Console;

public class HistoryCommand : IConsoleCommand
{
    public string Name => "history";
    public string Description => "";
    public string Usage => "";
    public IEnumerable<string> Aliases => [];

    public async Task ExecuteAsync(CommandContext context)
    {
        var data = context.Services.GetRequiredService<IDataService>();

        var limit = GetLimit(context.Arguments, 1);

        var list = await data.GetSpeedTestHistoryAsync(limit);

        var table = new Table();
        table.AddColumn("Time");
        table.AddColumn("Ping");
        table.AddColumn("Down");
        table.AddColumn("Up");

        foreach (var item in list)
        {
            table.AddRow(
                item.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                $"{item.PingMs} ms",
                $"{item.DownloadMbps:F0}",
                $"{item.UploadMbps:F0}");
        }

        AnsiConsole.Write(table);
    }
}
