namespace InternetMonitor.Core.Configs.Options;

public class MonitorOptions
{
    public MonitorSettings Monitor { get; set; } = new();
    public ConnectivitySettings Connectivity { get; set; } = new();
    public SpeedTestSettings SpeedTest { get; set; } = new();
    public DatabaseSettings Database { get; set; } = new();
    public ExportSettings Export { get; set; } = new();
}
