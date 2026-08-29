using InternetMonitor.Core.Models;

namespace InternetMonitor.UI.Models;

public sealed class DashboardSnapshot
{
    public MonitorServiceStatus WorkerStatus { get; init; } = new();
    public ConnectivityResult? LatestConnectivity { get; init; }
    public SpeedTestResult? LatestSpeedTest { get; init; }
}
