using Internet_Monitor.Models;

namespace Internet_Monitor.Interfaces;

public interface IDataService
{
    Task InitializeAsync();
    Task SaveConnectivityAsync(ConnectivityResult result);
    Task SaveSpeedTestAsync(SpeedTestResult result);

    Task<List<ConnectivityResult>> GetConnectivityHistoryAsync(int limit = 100);
    Task<List<SpeedTestResult>> GetSpeedTestHistoryAsync(int limit = 100);

    Task<ConnectivityResult?> GetLatestConnectivityAsync();
    Task<SpeedTestResult?> GetLatestSpeedTestAsync();
}
