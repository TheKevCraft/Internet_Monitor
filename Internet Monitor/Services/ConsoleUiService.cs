using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Internet_Monitor.Services;

public class ConsoleUiService
{
    private readonly IServiceProvider _provider;

    public ConsoleUiService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task RunAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Console.WriteLine("\n>");

            var input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            switch (input)
            {
                case "help":
                    ShowHelp();
                    break;
                case "status":
                    await ShowStatus();
                    break;
                case "speedtest":
                    await RunSpeedTest();
                    break;
                case "history":
                    await ShowHistory();
                    break;
                case "clear":
                    AnsiConsole.Clear();
                    break;
                case "export":
                    await HandleExport();
                    break;
                case "exit":
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Unknown command[/]");
                    break;
            }
        }
    }

    private void ShowHelp()
    {
        var table = new Table();
        table.AddColumn("Command");
        table.AddColumn("Description");

        table.AddRow("status", "Shows latest connectivity + speedtest");
        table.AddRow("speedtest", "Runs manual Ookla speedtest");
        table.AddRow("history", "Shows last 10 speedtests");
        table.AddRow("clear", "Clears screen");
        table.AddRow("exit", "Stops application");

        AnsiConsole.Write(table);
    }

    private async Task ShowStatus()
    {
        using var scope = _provider.CreateScope();

        var data = scope.ServiceProvider.GetRequiredService<IDataService>();

        var conn = await data.GetLatestConnectivityAsync();
        var speed = await data.GetLatestSpeedTestAsync();

        /*var panel = new Panel(
            $"""
            [green]Status:[/] {(conn?.IsOnline == true ? "Online" : "Offline")}

            Ping: {conn?.PingMs ?? 0} ms

            Download: {(speed?.DownloadMbps ?? 0):F1} Mbps
            Upload: {(speed?.UploadMbps ?? 0):F1} Mbps

            ISP: {speed?.Isp ?? ""}
            Server: {speed?.ServerName ?? ""}
            """);

        panel.Header = new PanelHeader("Internet Monitor");
        panel.Border = BoxBorder.Rounded;
        AnsiConsole.Write(panel);*/

        var download = speed?.DownloadMbps ?? 0;
        var upload = speed?.UploadMbps ?? 0;

        var table = new Table()
            //.Border(BoxBorder.Rounded)
            .AddColumn("Metric")
            .AddColumn("Value");

        table.AddRow("Status", conn?.IsOnline == true ? "[green]Online[/]" : "[red]Offline[/]");
        table.AddRow("Ping", $"{conn?.PingMs ?? 0} ms");
        table.AddRow("Download", $"{download:F1} Mbps");
        table.AddRow("Upload", $"{upload:F1} Mbps");
        table.AddRow("ISP", speed?.Isp ?? "-");
        table.AddRow("Server", speed?.ServerName ?? "-");

        AnsiConsole.Write(table);
    }

    private async Task RunSpeedTest()
    {
        using var scope = _provider.CreateScope();

        var speed = scope.ServiceProvider.GetRequiredService<ISpeedTestService>();
        var data = scope.ServiceProvider.GetRequiredService<IDataService>();

        await AnsiConsole.Status()
            .Start("Running speedtest...", async ctx =>
            {
                var result = await speed.RunAsync();

                await data.SaveSpeedTestAsync(result);

                ctx.Status("Done");

                AnsiConsole.MarkupLine(
                    $"[green]Download:[/] {result.DownloadMbps:F1} Mbps");
            });
    }

    private async Task ShowHistory()
    {
        using var scope = _provider.CreateScope();

        var data = scope.ServiceProvider.GetRequiredService<IDataService>();

        var list = await data.GetSpeedTestHistoryAsync(10);

        var table = new Table();
        table.AddColumn("Time");
        table.AddColumn("Ping");
        table.AddColumn("Down");
        table.AddColumn("Up");

        foreach (var item in list)
        {
            table.AddRow(
                item.Timestamp.ToString("HH:mm:ss"),
                $"{item.PingMs} ms",
                $"{item.DownloadMbps:F0}",
                $"{item.UploadMbps:F0}");
        }

        AnsiConsole.Write(table);
    }

    private async Task HandleExport()
    {
        using var scope = _provider.CreateScope();

        var export = scope.ServiceProvider.GetRequiredService<IExportService>();

        AnsiConsole.MarkupLine("[yellow]Format (csv/json):[/] ");
        var formatInput = Console.ReadLine()?.ToLower();

        var format = formatInput == "json"
            ? ExportFormat.Json
            : ExportFormat.Csv;

        AnsiConsole.MarkupLine("[yellow]Type (speed/connectivity):[/]");
        var type = Console.ReadLine()?.ToLower();

        AnsiConsole.MarkupLine("[yellow]Limit:[/]");
        var limitInput = Console.ReadLine();

        var limit = int.TryParse(limitInput, out var l) ? l : 100;

        string path;

        if (type == "speed")
        {
            path = await export.ExportSpeedTestsAsync(format, limit);
        }
        else
        {
            path = await export.ExportConnectivityAsync(format, limit);
        }

        AnsiConsole.MarkupLine($"[green]Export:[/] {path}");
    }
}
