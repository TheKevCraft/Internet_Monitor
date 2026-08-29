namespace InternetMonitor.Core.Models;

public sealed class MonitorEvent
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public MonitorEventType Type { get; set; }
    public MonitorEventSeverity Severity { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}
