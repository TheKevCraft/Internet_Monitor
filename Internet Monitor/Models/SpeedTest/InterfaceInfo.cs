using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Internet_Monitor.Models.SpeedTest;

public class InterfaceInfo
{
    [JsonPropertyName("externalIp")]
    public string? ExternalIp { get; set; }

    [JsonPropertyName("internalIp")]
    public string? InternalIp { get; set; }

    [JsonPropertyName("isVpn")]
    public bool IsVpn { get; set; }
}
