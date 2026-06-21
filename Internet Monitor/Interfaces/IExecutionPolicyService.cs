using Internet_Monitor.Models;

namespace Internet_Monitor.Interfaces;

public interface IExecutionPolicyService
{
    bool ShouldRunSpeedTest(ConnectivityResult? lastConnectivity);
}
