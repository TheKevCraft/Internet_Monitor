using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IReportService
{
    Task<InternetReport> GenerateReportAsync(int hours);
    Task<string> GenerateDailyReportAsync(int lastHours = 24);
}
