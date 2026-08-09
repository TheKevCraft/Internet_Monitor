using System.Text.Json.Serialization;

namespace InternetMonitor.Core.Models.SpeedTest;

public class ServerInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("host")]
    public string? Host { get; set; }
}
