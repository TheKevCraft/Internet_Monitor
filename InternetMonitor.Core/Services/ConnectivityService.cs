using InternetMonitor.Core.Configs.Options;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.NetworkInformation;

namespace InternetMonitor.Core.Services;

public class ConnectivityService : IConnectivityService
{
    private readonly HttpClient _httpClient;
    private readonly ConnectivitySettings _options;
    private readonly ILogger<ConnectivityService> _logger;

    public ConnectivityService(HttpClient httpClient, MonitorOptions options, ILogger<ConnectivityService> logger)
    {
        _httpClient = httpClient;
        _options = options.Connectivity;
        _logger = logger;
    }

    public async Task<ConnectivityResult> CheckAsync(CancellationToken cancellationToken = default)
    {
        var pingResult = await CheckPingAsync();
        var dnsResult = await CheckDnsAsync(cancellationToken);
        var httpResult = await CheckHttpAsync(cancellationToken);

        return new ConnectivityResult
        {
            Timestamp = DateTime.UtcNow,
            
            PingSuccess = pingResult.Success,
            PingMs = pingResult.PingMs,

            DnsSuccess = dnsResult,

            HttpSuccess = httpResult
        };
    }

    private async Task<(bool Success, long? PingMs)> CheckPingAsync()
    {
        bool success = false;
        long? pingMs = null;
        try
        {
            using var ping = new Ping();

            var reply = await ping.SendPingAsync(
                _options.PingHost,
                TimeSpan.FromMilliseconds(_options.PingTimeout));

            success = reply.Status == IPStatus.Success;

            if (success)
                pingMs = reply.RoundtripTime;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Ping connectivity check failed.");
            success = false;
        }

        return (success, pingMs);
    }

    private async Task<bool> CheckDnsAsync(CancellationToken cancellationToken)
    {
        bool result;
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(
                _options.DnsHost,
                cancellationToken);

            result = addresses.Length > 0;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "DNS connectivity check failed.");
            result = false;
        }        

        return result;
    }

    private async Task<bool> CheckHttpAsync(CancellationToken cancellationToken)
    {
        bool result;
        try
        {
            using var response = await _httpClient.GetAsync(
                _options.HttpUrl,
                cancellationToken);

            result = response.IsSuccessStatusCode;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "HTTP connectivity check failed.");
            result = false;
        }

        return result;
    }
}
