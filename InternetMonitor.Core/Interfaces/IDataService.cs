using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IDataService
{
    Task SaveConnectivityAsync(ConnectivityResult result, CancellationToken token = default);
    Task SaveSpeedTestAsync(SpeedTestResult result, CancellationToken token = default);

    Task<List<ConnectivityResult>> GetConnectivityHistoryAsync(int limit = 100, CancellationToken token = default);
    Task<List<SpeedTestResult>> GetSpeedTestHistoryAsync(int limit = 100, CancellationToken token = default);

    Task<ConnectivityResult?> GetLatestConnectivityAsync();
    Task<SpeedTestResult?> GetLatestSpeedTestAsync();
}

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}
