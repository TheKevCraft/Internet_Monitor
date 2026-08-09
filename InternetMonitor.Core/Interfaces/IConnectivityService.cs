using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IConnectivityService
{
    Task<ConnectivityResult> CheckAsync(CancellationToken cancellationToken = default);
}
