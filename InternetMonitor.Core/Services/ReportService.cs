using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using System.Text;

namespace InternetMonitor.Core.Services;

public class ReportService : IReportService
{
    private readonly IDataService _dataService;

    public ReportService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<InternetReport> GenerateReportAsync(int hours)
    {
        var since = DateTime.UtcNow.AddHours(-hours);

        var connectivity = await _dataService.GetConnectivityHistoryAsync(1000);
        var speedtests = await _dataService.GetSpeedTestHistoryAsync(1000);

        var filteredConn = connectivity.Where(x => x.Timestamp >= since).ToList();
        var filteredSpeed = speedtests.Where(X => X.Timestamp >= since).ToList();

        var total = filteredConn.Count;

        if (total == 0)
            return new InternetReport
            {
                GeneratedAt = DateTime.UtcNow
            };

        var pingValues = filteredConn
            .Where(x => x.PingMs.HasValue)
            .Select(X => X.PingMs!.Value)
            .ToList();

        return new InternetReport
        {
            GeneratedAt = DateTime.UtcNow,

            UptimePercent = filteredConn.Count(x => x.IsOnline) * 100.0 / total,

            AveragePing = pingValues.Count > 0
                ? pingValues.Average()
                : 0,

            AverageDownload = filteredSpeed.Count > 0
                ? filteredSpeed.Average(x => x.DownloadMbps)
                : 0,

            AverageUpload = filteredSpeed.Count > 0
                ? filteredSpeed.Average(x => x.UploadMbps)
                : 0,

            OfflineEvents = filteredConn.Count(x => !x.IsOnline),

            SpeedTestCount = filteredSpeed.Count
        };
    }

    public async Task<string> GenerateDailyReportAsync(int lasthours = 24)
    {
        var report = await GenerateReportAsync(lasthours);

        var sb = new StringBuilder();

        sb.AppendLine("Internet Monitor Report");
        sb.AppendLine($"Period: last {lasthours} hours");
        sb.AppendLine("");

        sb.AppendLine("Status:");
        sb.AppendLine($"Online: {report.UptimePercent:F1}%");
        sb.AppendLine($"Offline: {report.OfflineEvents:F1}%");
        sb.AppendLine("");

        sb.AppendLine("Performance:");
        sb.AppendLine($"Avg Ping: {report.AveragePing:F1} ms");
        sb.AppendLine($"Avg Download: {report.AverageDownload:F1} Mbps");
        sb.AppendLine($"Avg Upload: {report.AverageUpload:F1} Mbps");
        sb.AppendLine("");

        sb.AppendLine($"Speedtests: {report.SpeedTestCount}");

        return sb.ToString();
    }
}
