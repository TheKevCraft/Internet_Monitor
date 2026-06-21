namespace Internet_Monitor.Models;

public class SpeedTestResult
{
    public DateTime Timestamp { get; set; }

    public double PingMs { get; set; }
    public double Jitter { get; set; }

    public double DownloadMbps { get; set; }
    public double UploadMbps { get; set; }
    public bool PacketLoss { get; set; }

    public string? PublicIp { get; set; }
    public string? Isp { get; set; }

    public string? ServerName { get; set; }
    public string? ServerLocation { get; set; }

    public bool Success { get; set; }
    public string? Error { get; set; }
}
