using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IExporter
{
    ExportFormat Format { get; }

    Task<string> ExportAsync(InternetExportData data, CancellationToken token = default);
}