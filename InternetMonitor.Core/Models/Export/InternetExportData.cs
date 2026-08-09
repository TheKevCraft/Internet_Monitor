namespace InternetMonitor.Core.Models;

public class InternetExportData
{
    public DateTime GeneratedAt { get; set; }

    public InternetReport Summary { get; set; } = new();

    public IEnumerable<ConnectivityResult> Connectivity { get; set; } = [];

    public IEnumerable<SpeedTestResult> SpeedTests { get; set; } = [];
}