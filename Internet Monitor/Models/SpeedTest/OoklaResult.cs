using System.Text.Json.Serialization;

namespace Internet_Monitor.Models.SpeedTest;

public class OoklaResult
{
    [JsonPropertyName("ping")]
    public PingInfo Ping { get; set; } = new();

    [JsonPropertyName("download")]
    public TransferInfo Download { get; set; } = new();

    [JsonPropertyName("upload")]
    public TransferInfo Upload { get; set; } = new();

    [JsonPropertyName("interface")]
    public InterfaceInfo Interface { get; set; } = new();

    [JsonPropertyName("isp")]
    public string? Isp { get; set; }

    [JsonPropertyName("packetLoss")]
    public double PacketLoss { get; set; }

    [JsonPropertyName("server")]
    public ServerInfo? Server { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}
