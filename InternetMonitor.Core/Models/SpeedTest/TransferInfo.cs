using System.Text.Json.Serialization;

namespace InternetMonitor.Core.Models.SpeedTest;

public class TransferInfo
{
    [JsonPropertyName("bandwidth")]
    public long Bandwidth { get; set; }

    [JsonPropertyName("bytes")]
    public long Bytes { get; set; }

    [JsonPropertyName("elapsed")]
    public long Elapsed { get; set; }
}
