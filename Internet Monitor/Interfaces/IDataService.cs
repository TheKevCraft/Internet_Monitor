using Internet_Monitor.Models;

namespace Internet_Monitor.Interfaces;

public interface IDataService
{
    Task InitializeAsync();
    Task SaveConnectivityAsync(ConnectivityResult result);
    Task SaveSpeedTestAsync(SpeedTestResult result);
}
