namespace InternetMonitor.Core.Configs.Options;

public class SpeedTestSettings
{
    public bool Enabled { get; set; } = true;
    public int Interval { get; set; } = 3600;
    public string ToolPath { get; set; } = "";
}
