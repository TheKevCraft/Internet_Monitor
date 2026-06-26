using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Internet_Monitor.Commands.Console;

public class StatusCommand : IConsoleCommand
{
    public string Name => "status";
    public string Usage => "";
    public string Description => "";
    public IEnumerable<string> Aliases => [];
    
    public async Task ExecuteAsync(CommandContext context)
    {
        var data = context.Services.GetRequiredService<IDataService>();

        var conn = await data.GetLatestConnectivityAsync();
        var speed = await data.GetLatestSpeedTestAsync();

        var download = speed?.DownloadMbps ?? 0;
        var upload = speed?.UploadMbps ?? 0;

        var table = new Table()
            .AddColumn("Metric")
            .AddColumn("Value");

        table.AddRow("Status", conn?.IsOnline == true ? "[green]Online[/]" : "[red]Offline[/]");
        table.AddRow("Ping", conn?.PingMs is not null ? $"{conn.PingMs} ms" : "-");
        table.AddRow("Download", $"{download:F1} Mbps");
        table.AddRow("Upload", $"{upload:F1} Mbps");
        table.AddRow("ISP", speed?.Isp ?? "-");
        table.AddRow("Server", speed?.ServerName ?? "-");

        AnsiConsole.Write(table);
    }
}
