namespace InternetMonitor.Core.Models;

public enum MonitorEventType
{
    ServiceStarted,
    ServiceStopped,

    ConnectivityCheck,
    ConnectivityLost,
    ConnectivityRestored,

    SpeedTestStarted,
    SpeedTestCompleted,
    SpeedTestFailed,

    DatabaseError,
    ConfigurationError
}
