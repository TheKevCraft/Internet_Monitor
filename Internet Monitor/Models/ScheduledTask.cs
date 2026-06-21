namespace Internet_Monitor.Models;

public class ScheduledTask
{
    public string Name { get; set; } = "";
    public TimeSpan Interval { get; set; }
    public DateTime LastRun { get; set; }
    public Func<Task> Action { get; set; } = default!;
}
