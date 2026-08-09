namespace InternetMonitor.Core.Config;

public class ConnectivityOptions
{
    public string PingHost { get; set; } = "1.1.1.1";
    public string DnsHost { get; set; } = "google.com";
    public string HttpUrl { get; set; } = "https://connectivitycheck.gstatic.com/generate_204";
    public int PingTimeout { get; set; } = 3000;
}