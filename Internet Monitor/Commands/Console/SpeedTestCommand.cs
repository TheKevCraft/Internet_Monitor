using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Internet_Monitor.Commands.Console;

public class SpeedTestCommand : IConsoleCommand
{
    public string Name => "speedtest";
    public string Description => "";
    public string Usage => "";
    public IEnumerable<string> Aliases => [];

    public async Task ExecuteAsync(CommandContext context)
    {
        var speed = context.Services.GetRequiredService<ISpeedTestService>();
        var data = context.Services.GetRequiredService<IDataService>();

        System.Console.WriteLine("Starting speedtest...");

        var result = await speed.RunAsync();

        await data.SaveSpeedTestAsync(result);

        System.Console.WriteLine();
        System.Console.WriteLine($"Download: {result.DownloadMbps:F1} Mbps");
        System.Console.WriteLine($"Upload: {result.UploadMbps:F1} Mbps");
        System.Console.WriteLine($"Ping: {result.PingMs:F1} ms");
    }
}
