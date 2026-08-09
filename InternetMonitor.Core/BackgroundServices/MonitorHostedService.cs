using InternetMonitor.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace InternetMonitor.Core.BackgroundServices;

public class MonitorHostedService : BackgroundService
{
    private readonly IMonitorService _monitor;

    public MonitorHostedService(IMonitorService monitor)
    {
        _monitor = monitor;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return _monitor.RunAsync(stoppingToken);
    }
}