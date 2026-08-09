namespace InternetMonitor.Core.Models;

public class ScheduledTask
{
    public string Name { get; set; } = "";
    public TimeSpan Interval { get; set; }
    public DateTime LastRun { get; set; }
    public Func<Task> Action { get; set; } = default!;

    public Func<TimeSpan, TimeSpan>? IntervalStrategy { get; set; }
}
