namespace InternetMonitor.Core.Models;

public class InternetReport
{
    public DateTime GeneratedAt { get; set; }
    public double UptimePercent { get; set; }
    public double AveragePing { get; set; }
    public double AverageDownload { get; set; }
    public double AverageUpload { get; set; }
    public int OfflineEvents { get; set; }
    public int SpeedTestCount { get; set; }
}
