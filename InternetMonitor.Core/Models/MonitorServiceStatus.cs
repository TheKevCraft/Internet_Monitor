namespace InternetMonitor.Core.Models;

public class MonitorServiceStatus
{
    public bool IsRunning { get; init; }
    public string Status { get; init; } = "Unknown";
    public string? Details { get; init; }
}
