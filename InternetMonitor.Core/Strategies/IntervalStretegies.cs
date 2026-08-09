using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Strategies;

internal static class IntervalStretegies
{
    public static TimeSpan SpeedTestStrategy(TimeSpan baseInterval, ConnectivityResult? conn)
    {
        if (conn is null)
            return TimeSpan.FromHours(1);

        if (!conn.IsOnline)
            return TimeSpan.FromHours(2);

        if (conn.PingMs > 200)
            return TimeSpan.FromMinutes(10);

        // gute Verbindung -> weniger testen
        return TimeSpan.FromMinutes(30);
    }
}
