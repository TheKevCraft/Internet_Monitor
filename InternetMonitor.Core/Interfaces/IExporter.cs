using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IExporter
{
    ExportFormat Format { get; }

    Task<ExportResult> ExportAsync(
        InternetExportData data, 
        CancellationToken token = default);
}