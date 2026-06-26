using Internet_Monitor.Models;

namespace Internet_Monitor.Interfaces;

public interface IReportService
{
    Task<InternetReport> GenerateReportAsync(int hours);
    Task<string> GenerateDailyReportAsync(int lastHours = 24);
}
