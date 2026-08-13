using InternetMonitor.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InternetMonitor.Core.BackgroundServices;

public sealed class InternetMonitorWorker : BackgroundService
{
    private readonly IConnectivityService _connectivityService;
    private readonly IExecutionPolicyService _executionPolicyService;
    private readonly ISpeedTestService _speedTestService;
    private readonly IDataService _dataService;
    private readonly ILogger<InternetMonitorWorker> _logger;

    public InternetMonitorWorker(
        IConnectivityService connectivityService,
        IExecutionPolicyService executionPolicyService,
        ISpeedTestService speedTestService,
        IDataService dataService,
        ILogger<InternetMonitorWorker> logger)
    {
        _connectivityService = connectivityService;
        _executionPolicyService = executionPolicyService;
        _speedTestService = speedTestService;
        _dataService = dataService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Internet Monitor Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteMonitorCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during monitor cycle.");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(1),
                stoppingToken);
        }

        _logger.LogInformation("Internet Monitor worker stopped.");
    }

    private async Task ExecuteMonitorCycleAsync(CancellationToken token)
    {
        _logger.LogDebug("Starting monitor cycle.");

        var connectivity = await _connectivityService.CheckAsync(token);

        _logger.LogInformation(
            "Connectivity check completed. Online: {IsOnline}, Ping {PingMs} ms",
            connectivity.IsOnline,
            connectivity.PingMs);

        await _dataService.SaveConnectivityAsync(connectivity, token);

        if (_executionPolicyService.ShouldRunSpeedTest(connectivity))
        {
            _logger.LogInformation("Execution policy allows a speed test.");

            try
            {
                var speedtest = await _speedTestService.RunAsync(token);

                await _dataService.SaveSpeedTestAsync(speedtest, token);

                _logger.LogInformation(
                    "Speed Test completed. " +
                    "Download: {Downlaod} Mbps, " +
                    "Upload: {Upload} Mbps, " +
                    "Ping: {Ping} ms",
                    speedtest.DownloadMbps,
                    speedtest.UploadMbps,
                    speedtest.PingMs);
            }
            catch (OperationCanceledException)
                when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Speed test failed.");
            }
        }
        else
        {
            _logger.LogDebug("Execution policy does not require a speed test.");
        }
    }
}
