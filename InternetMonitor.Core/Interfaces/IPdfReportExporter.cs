using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IPdfReportExporter
{
    string ExportAsync(InternetReport report);
}
