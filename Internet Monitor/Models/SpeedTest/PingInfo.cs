using System.Text.Json.Serialization;

namespace Internet_Monitor.Models.SpeedTest;

public class PingInfo
{
    [JsonPropertyName("latency")]
    public double Latency { get; set; }

    [JsonPropertyName("jitter")]
    public double Jitter { get; set; }

    [JsonPropertyName("low")]
    public double Low { get; set; }

    [JsonPropertyName("high")]
    public double High { get; set; }
}
