using System.Runtime.InteropServices;
using InternetMonitor.Core.Interfaces;

namespace InternetMonitor.Core.Services;

public class SpeedTestCliLocator : ISpeedTestCliLocator
{
    public string GetPath()
    {
        var baseDir = AppContext.BaseDirectory;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(baseDir, "Tools", "speedtest", "windows", "speedtest.exe");
        }

        return Path.Combine(baseDir, "Tools", "speedtest", "linux", "speedtest");
    }
}
