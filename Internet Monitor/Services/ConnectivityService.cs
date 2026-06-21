using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using System.Net;
using System.Net.NetworkInformation;

namespace Internet_Monitor.Services;

public class ConnectivityService : IConnectivityService
{
    private readonly HttpClient _httpClient;

    public ConnectivityService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ConnectivityResult> CheckAsync(CancellationToken cancellationToken = default)
    {
        var result = new ConnectivityResult
        {
            Timestamp = DateTime.UtcNow,
        };

        // Ping
        try
        {
            using var ping = new Ping();

            var reply = await ping.SendPingAsync(
                "1.1.1.1",
                TimeSpan.FromSeconds(3));

            result.PingSuccess =
                reply.Status == IPStatus.Success;

            if (result.PingSuccess)
                result.PingMs = reply.RoundtripTime;
        }
        catch
        {
            result.PingSuccess = false;
        }

        // DNS
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(
                "google.com",
                cancellationToken);

            result.DnsSuccess = addresses.Length > 0;
        }
        catch
        {
            result.DnsSuccess = false;
        }

        // HTTP
        try
        {
            using var response = await _httpClient.GetAsync(
                "http://www.google.com",
                cancellationToken);

            result.HttpSuccess =
                response.IsSuccessStatusCode;
        }
        catch
        {
            result.HttpSuccess = false;
        }

        return result;
    }
}
