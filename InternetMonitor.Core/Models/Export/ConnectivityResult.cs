namespace InternetMonitor.Core.Models;

public class ConnectivityResult
{
    public DateTime Timestamp { get; set; }

    public bool PingSuccess { get; set; }
    public long? PingMs { get; set; }

    public bool DnsSuccess { get; set; }

    public bool HttpSuccess { get; set; }

    public bool IsOnline =>
        PingSuccess &&
        DnsSuccess &&
        HttpSuccess;
}
