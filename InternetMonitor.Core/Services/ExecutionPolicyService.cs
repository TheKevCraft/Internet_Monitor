using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Services;

public class ExecutionPolicyService : IExecutionPolicyService
{
    public bool ShouldRunSpeedTest(ConnectivityResult? lastConnectivity)
    {
        // 1. Kein Messwert vorhanden
        if (lastConnectivity == null)
            return false;

        // 2. Komplett offline
        if (!lastConnectivity.IsOnline)
            return false;

        // 3. Kein Ping moglich
        if (!lastConnectivity.PingSuccess)
            return false;

        // 4. Ping zu hoch -> optimaler Schutz
        if (lastConnectivity.PingMs.HasValue &&
            lastConnectivity.PingMs.Value > 300)
            return false;

        return true;
    }
}
