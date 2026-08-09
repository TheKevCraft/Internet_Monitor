using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface ISpeedTestService
{
    Task<SpeedTestResult> RunAsync(CancellationToken cancellationToken = default);
}
