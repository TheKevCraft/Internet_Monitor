using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IExecutionPolicyService
{
    bool ShouldRunSpeedTest(ConnectivityResult? lastConnectivity);
}
