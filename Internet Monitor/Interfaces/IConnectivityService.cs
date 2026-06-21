using Internet_Monitor.Models;

namespace Internet_Monitor.Interfaces;

public interface IConnectivityService
{
    Task<ConnectivityResult> CheckAsync(CancellationToken cancellationToken = default);
}
