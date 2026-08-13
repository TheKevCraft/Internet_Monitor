using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IMonitorServiceController
{
    Task InstallAsync(CancellationToken token = default);
    Task UninstallAsync(CancellationToken token = default);

    Task StartAsync(CancellationToken token = default);
    Task StopAsync(CancellationToken token = default);
    Task RestartAsync(CancellationToken token = default);
    Task<MonitorServiceStatus> GetStatusAsync(CancellationToken token = default);
    Task<IReadOnlyList<string>> GetLogsAsync(
        int limit = 50,
        CancellationToken token = default);
}
