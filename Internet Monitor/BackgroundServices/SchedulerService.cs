using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Internet_Monitor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace Internet_Monitor.BackgroundServices;

public class SchedulerService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IDataService _dataService;
    private readonly List<ScheduledTask> _tasks = new();

    public SchedulerService(IServiceProvider provider, IDataService dataService)
    {
        _provider = provider;
        _dataService = dataService;

        // Tasks registieren
        _tasks.Add(new ScheduledTask
        {
            Name = "Connectvity",
            Interval = TimeSpan.FromMinutes(1),
            LastRun = DateTime.MinValue,
            Action = RunConnectivityCheck
        });

        _tasks.Add(new ScheduledTask
        {
            Name = "SpeedTest",
            Interval = TimeSpan.FromMinutes(30),
            LastRun = DateTime.MinValue,
            Action = RunSpeedTest
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            foreach (var task in _tasks)
            {
                if (now - task.LastRun >= task.Interval)
                {
                    task.LastRun = now;

                    try
                    {
                        await task.Action();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Scheduler] {task.Name} failed: {ex.Message}");
                    }
                }
            }
        }

        await Task.Delay(1000, stoppingToken);
    }

    private async Task RunConnectivityCheck()
    {
        using var scope = _provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IConnectivityService>();

        var result = await service.CheckAsync();

        Console.WriteLine($"[Connectivity] Online={result.IsOnline}, Ping={result.PingMs}");

        await _dataService.SaveConnectivityAsync(result);
    }

    private async Task RunSpeedTest()
    {
        using var scope = _provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<ISpeedTestService>();

        var result = await service.RunAsync();

        Console.WriteLine($"[SpeedTest] DL={result.DownloadMbps:F1} Mbps, UL={result.UploadMbps:F1} Mbps");

        await _dataService.SaveSpeedTestAsync(result);
    }
}
