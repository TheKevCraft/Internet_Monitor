using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using System.Runtime.Versioning;
using System.ServiceProcess;

namespace InternetMonitor.CLI.Services;

[SupportedOSPlatform("windows")]
internal class WindowsMonitorServiceController : IMonitorServiceController
{
    private const string ServiceName = "InternetMonitor";

    #region install

    public Task InstallAsync(CancellationToken token = default) { return Task.CompletedTask; }

    public Task UninstallAsync(CancellationToken token = default) { return Task.CompletedTask; }

    #endregion

    #region Controls
    
    public Task StartAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        using var service = new ServiceController(ServiceName);

        service.Refresh();

        if (service.Status == ServiceControllerStatus.Running)
            return Task.CompletedTask;

        service.Start();
        service.WaitForStatus(
            ServiceControllerStatus.Running,
            TimeSpan.FromSeconds(30));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        using var service = new ServiceController(ServiceName);

        service.Refresh();

        if (service.Status == ServiceControllerStatus.Stopped)
            return Task.CompletedTask;

        service.Stop();
        service.WaitForStatus(
            ServiceControllerStatus.Stopped,
            TimeSpan.FromSeconds(30));

        return Task.CompletedTask;
    }

    public async Task RestartAsync(CancellationToken token = default)
    {
        await StopAsync(token);
        await StartAsync(token);
    }

    public Task<MonitorServiceStatus> GetStatusAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        using var service = new ServiceController(ServiceName);

        service.Refresh();

        var status = service.Status;

        return Task.FromResult(
            new MonitorServiceStatus
            {
                IsRunning = status == ServiceControllerStatus.Running,
                Status = status.ToString(),
                Details = $"Windows Service: {ServiceName}"
            });
    }

    public Task<IReadOnlyList<string>> GetLogsAsync(
        int lines = 50,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        // Wird mit dem Logging/EventLog-System erganzt.
        IReadOnlyList<string> logs =
            [
                "Service logging is not implemented yet."
            ];

        return Task.FromResult(logs);
    }

    #endregion
}
