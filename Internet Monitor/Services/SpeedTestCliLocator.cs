using System.Runtime.InteropServices;

namespace Internet_Monitor.Services;

public static class SpeedTestCliLocator
{
    public static string GetPath()
    {
        var baseDir = AppContext.BaseDirectory;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(baseDir, "Tools", "speedtest", "windows", "speedtest.exe");
        }

        return Path.Combine(baseDir, "Tools", "speedtest", "linux", "speedtest");
    }
}
