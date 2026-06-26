using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using System.Text;

namespace Internet_Monitor.Services;

public class ReportService : IReportService
{
    private readonly IDataService _dataService;

    public ReportService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<InternetReport> GenerateReportAsync(int hours)
    {
        var report = new InternetReport();

        report.GeneratedAt = DateTime.Now;

        // Daten Berechnen

        return report;
    }

    public async Task<string> GenerateDailyReportAsync(int lasthours = 24)
    {
        var since = DateTime.UtcNow.AddHours(-lasthours);

        var connectivity = await _dataService.GetConnectivityHistoryAsync(1000);
        var speedtests = await _dataService.GetSpeedTestHistoryAsync(1000);

        Console.WriteLine($"Connectivity: {connectivity.Count}");
        Console.WriteLine($"Speedtests: {speedtests.Count}");

        var filteredConn = connectivity.Where(x => x.Timestamp >= since).ToList();
        var filteredSpeed = speedtests.Where(x => x.Timestamp >= since).ToList();

        // --- Calculations ---
        var total = filteredConn.Count();

        if (total == 0)
        {
            return
                $"Internet Monitor Report\n" +
                $"Period: last {lasthours} hours\n\n" +
                "No connectivity data available.";
        }

        var online = filteredConn.Count(x => x.IsOnline);
        var offline = total - online;

        double avgPing = filteredConn
            .Where(x => x.PingMs.HasValue)
            .Select(x => x.PingMs!.Value)
            .DefaultIfEmpty(0)
            .Average();

        double avgDown = filteredSpeed
            .Select(x => x.DownloadMbps)
            .DefaultIfEmpty(0)
            .Average();

        double avgUp = filteredSpeed
            .Select(x => x.UploadMbps)
            .DefaultIfEmpty(0)
            .Average();

        var sb = new StringBuilder();

        sb.AppendLine("Internet Monitor Report");
        sb.AppendLine($"Period: last {lasthours} hours");
        sb.AppendLine("");

        sb.AppendLine("Status:");
        sb.AppendLine($"Online: {(online / (double)total * 100):F1}%");
        sb.AppendLine($"Offline: {(offline / (double)total * 100):F1}%");
        sb.AppendLine("");

        sb.AppendLine("Performance:");
        sb.AppendLine($"Avg Ping: {avgPing:F1} ms");
        sb.AppendLine($"Avg Download: {avgDown:F1} Mbps");
        sb.AppendLine($"Avg Upload: {avgUp:F1} Mbps");
        sb.AppendLine("");

        sb.AppendLine($"Speedtests: {filteredSpeed.Count}");

        return sb.ToString();
    }
}
