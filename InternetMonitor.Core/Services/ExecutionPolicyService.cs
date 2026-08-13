using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Services;

/// <summary>
/// Service that decides, based on the latest connectivity measurement, whether a speed test should be executed.
/// </summary>
internal class ExecutionPolicyService : IExecutionPolicyService
{
    /// <summary>
    /// Determines whether a speed test should be run.
    /// </summary>
    /// <param name="connectivity">
    /// The last known connectivity measurement. May be <c>null</c> if no measurement is available.
    /// </param>
    /// <returns>
    /// <c>true</c> if a speed test should be executed; otherwise <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Decision policy:
    /// 1. No measurement available (<paramref name="connectivity"/> == <c>null</c>) -> do not run.
    /// 2. Completely offline (<see cref="ConnectivityResult.IsOnline"/> == <c>false</c>) -> do not run.
    /// 3. Ping failed (<see cref="ConnectivityResult.PingSuccess"/> == <c>false</c>) -> do not run.
    /// 4. Ping too high (<see cref="ConnectivityResult.PingMs"/> &gt; 300 ms) -> do not run (protects from unnecessary load).
    /// If none of these conditions apply, a speed test is allowed.
    /// </remarks>
    public bool ShouldRunSpeedTest(ConnectivityResult? connectivity)
    {
        // 1. No measurement available
        if (connectivity == null)
            return false;

        // 2. Completely offline
        if (!connectivity.IsOnline)
            return false;

        // 3. Ping failed
        if (!connectivity.PingSuccess)
            return false;

        // 4. Ping too high -> protective guard
        if (connectivity.PingMs.HasValue &&
            connectivity.PingMs.Value > 300)
            return false;

        return true;
    }
}
