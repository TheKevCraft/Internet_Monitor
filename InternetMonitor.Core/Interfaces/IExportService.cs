using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IExportService
{
    Task<ExportResult> ExportAsync(
        ExportFormat format, 
        int limit, 
        CancellationToken token = default);
}
