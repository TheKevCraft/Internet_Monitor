using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IExportService
{
    Task<string> ExportAsync(ExportFormat format, int limit, CancellationToken token = default);
}
