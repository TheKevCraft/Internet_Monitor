using Internet_Monitor.Models;

namespace Internet_Monitor.Interfaces;

public interface IExportService
{
    Task<string> ExportSpeedTestsAsync(ExportFormat format, int limit);
    Task<string> ExportConnectivityAsync(ExportFormat format, int limit);
}
