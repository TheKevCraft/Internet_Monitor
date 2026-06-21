using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Internet_Monitor.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            Action = RunSpeedTest,
            IntervalStrategy = null // wird automatisch gesetzt
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var lastConn = await _dataService.GetLatestConnectivityAsync();

            foreach (var task in _tasks)
            {
                var interval = task.Interval;

                // dynamische Anpasseung
                if (task.Name == "SpeedTest")
                {
                    interval = IntervalStretegies.SpeedTestStrategy(interval, lastConn);
                }

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
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task RunConnectivityCheck()
    {
        using var scope = _provider.CreateScope();

        var data = scope.ServiceProvider.GetRequiredService<IDataService>();
        var service = scope.ServiceProvider.GetRequiredService<IConnectivityService>();

        var result = await service.CheckAsync();

        //Console.WriteLine($"[Connectivity] Online={result.IsOnline}, Ping={result.PingMs}");

        await data.SaveConnectivityAsync(result);
    }

    private async Task RunSpeedTest()
    {
        using var scope = _provider.CreateScope();

        var data = scope.ServiceProvider.GetRequiredService<IDataService>();
        var speed = scope.ServiceProvider.GetRequiredService<ISpeedTestService>();
        var policy = scope.ServiceProvider.GetRequiredService<IExecutionPolicyService>();

        var lastConnection = await data.GetLatestConnectivityAsync();

        // Skip-Logic
        if (!policy.ShouldRunSpeedTest(lastConnection))
        {
            Console.WriteLine("[Scheduler] Speedtest skipped by policy");
            return;
        }

        var result = await speed.RunAsync();

        //Console.WriteLine($"[SpeedTest] DL={result.DownloadMbps:F1} Mbps, UL={result.UploadMbps:F1} Mbps");

        await data.SaveSpeedTestAsync(result);
    }
}
