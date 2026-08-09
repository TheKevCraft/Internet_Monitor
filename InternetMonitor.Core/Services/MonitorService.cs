using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using InternetMonitor.Core.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace InternetMonitor.Core.Services;

public class MonitorService : IMonitorService
{
    private readonly IServiceProvider _provider;
    private readonly IDataService _dataService;

    private readonly List<ScheduledTask> _tasks = new();

    public MonitorService(IServiceProvider provider, IDataService dataService)
    {
        _provider = provider;
        _dataService = dataService;

        _tasks.Add(new ScheduledTask
        {
            Name = "Connectivity",
            Interval = TimeSpan.FromMinutes(1),
            LastRun = DateTime.MinValue,
            Action = RunConnectivityCheck
        });

        _tasks.Add(new ScheduledTask
        {
            Name = "Speedtest",
            Interval = TimeSpan.FromMinutes(30),
            LastRun = DateTime.MinValue,
            Action = RunSpeedTest
        });
    }

    public async Task RunAsync(CancellationToken token)
    {
        Console.WriteLine("[Monitor] Stated.");

        while (!token.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var lastConn = await _dataService.GetLatestConnectivityAsync();

            foreach (var task in _tasks)
            {
                var interval = task.Interval;

                if (task.Name == "SpeedTest")
                {
                    interval = IntervalStretegies.SpeedTestStrategy(interval, lastConn);
                }

                if (now - task.LastRun >= interval)
                {
                    task.LastRun = now;

                    try
                    {
                        await task.Action();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Monitor] {task.Name} failed: {ex.Message}");
                    }
                }
            }

            await Task.Delay(1000, token);
        }
    }

    private async Task RunConnectivityCheck()
    {
        using var scope = _provider.CreateScope();

        var data = scope.ServiceProvider.GetRequiredService<IDataService>();
        var service = scope.ServiceProvider.GetRequiredService<IConnectivityService>();

        var result = await service.CheckAsync();

        await data.SaveConnectivityAsync(result);
    }

    private async Task RunSpeedTest()
    {
        using var scope = _provider.CreateScope();

        var data  = scope.ServiceProvider.GetRequiredService<IDataService>();
        var speed = scope.ServiceProvider.GetRequiredService<ISpeedTestService>();
        var policy = scope.ServiceProvider.GetRequiredService<IExecutionPolicyService>();

        var lastConnection = await data.GetLatestConnectivityAsync();

        if (!policy.ShouldRunSpeedTest(lastConnection))
        {
            Console.WriteLine("[Monitor] Speedtest skipped by policy.");

            return;
        }

        var result = await speed.RunAsync();

        await data.SaveSpeedTestAsync(result);
    }
}