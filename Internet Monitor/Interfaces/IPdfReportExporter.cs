using Internet_Monitor.Models;

namespace Internet_Monitor.Interfaces;

public interface IPdfReportExporter
{
    Task<string> ExportAsync(InternetReport report);
}
