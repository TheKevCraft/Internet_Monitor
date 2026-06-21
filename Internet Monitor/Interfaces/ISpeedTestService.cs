using Internet_Monitor.Models;

namespace Internet_Monitor.Interfaces;

public interface ISpeedTestService
{
    Task<SpeedTestResult> RunAsync(CancellationToken cancellationToken = default);
}
