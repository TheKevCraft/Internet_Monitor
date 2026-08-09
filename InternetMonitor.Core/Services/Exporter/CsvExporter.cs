using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using System.Globalization;
using System.Text;

namespace InternetMonitor.Core.Services.Exporter;

internal class CsvExporter : IExporter
{
    public ExportFormat Format => ExportFormat.Csv;

    public async Task<ExportResult> ExportAsync(InternetExportData data, CancellationToken token = default)
    {
        var timestamp = DateTime.UtcNow.ToString(
            "yyyy-MM-dd_HH-mm-ss", 
            CultureInfo.InvariantCulture);

        var folder = CreateExportFolder();

        var summaryPath = Path.Combine(
            folder,
            $"InternetReport_{timestamp}_summary.csv");

        var connectivityPath = Path.Combine(
            folder,
            $"InternetReport_{timestamp}_connectivity.csv");

        var speedTestsPath = Path.Combine(
            folder,
            $"InternetReport_{timestamp}_speedtests.csv");

        await ExportSummaryAsync(
            summaryPath,
            data.Summary,
            token);

        await ExportConnectivityAsync(
            connectivityPath,
            data.Connectivity,
            token);

        await ExportSpeedTestsAsync(
            speedTestsPath,
            data.SpeedTests,
            token);

        return new ExportResult
        {
            Format = Format,
            GeneratedAt = data.GeneratedAt,
            Files = 
            [
                summaryPath,
                connectivityPath,
                speedTestsPath
            ]
        };
    }

    #region Export Tabels

    private static async Task ExportSummaryAsync(
        string filePath,
        InternetReport report,
        CancellationToken token)
    {
        var sb = new StringBuilder();

        sb.AppendLine(
            "GeneratedAt,UpTimePercent,AveragePing,AverageDownload,AverageUpload,OfflineEvents,SpeedTestCount");

        sb.AppendLine(string.Join(",",
            FormatDateTime(report.GeneratedAt),
            FormatNunber(report.UptimePercent),
            FormatNunber(report.AveragePing),
            FormatNunber(report.AverageDownload),
            FormatNunber(report.AverageUpload),
            report.OfflineEvents.ToString(CultureInfo.InvariantCulture),
            report.SpeedTestCount.ToString(CultureInfo.InvariantCulture)));

        await WriteFilesAsync(filePath, sb.ToString(), token);
    }

    private static async Task ExportConnectivityAsync(
        string filePath,
        IEnumerable<ConnectivityResult> results,
        CancellationToken token)
    {
        var sb = new StringBuilder();

        sb.AppendLine(
            "Timestamp,PingSuccess,PingMs,DnsSuccess,HttpSuccess,IsOnline");

        foreach (var result in results)
        {
            sb.AppendLine(string.Join(",",
                FormatDateTime(result.Timestamp),
                result.PingSuccess,
                result.PingMs?.ToString(CultureInfo.InvariantCulture) ?? "",
                result.DnsSuccess,
                result.HttpSuccess,
                result.IsOnline));
        }

        await WriteFilesAsync(filePath, sb.ToString(), token);
    }

    private static async Task ExportSpeedTestsAsync(
        string filePath,
        IEnumerable<SpeedTestResult> results,
        CancellationToken token)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Timestamp,PingMs,Jitter,DownloadMbps,UploadMbps,PacketLoss,PublicIp,ISP,ServerName,ServerLocation,Success,Error");

        foreach (var result in results)
        {
            sb.AppendLine(string.Join(",",
                FormatDateTime(result.Timestamp),
                FormatNunber(result.PingMs),
                FormatNunber(result.Jitter),
                FormatNunber(result.DownloadMbps),
                FormatNunber(result.UploadMbps),
                FormatNunber(result.PacketLoss),
                CsvEscape(result.PublicIp),
                CsvEscape(result.Isp),
                CsvEscape(result.ServerName),
                CsvEscape(result.ServerLocation),
                result.Success,
                CsvEscape(result.Error)));
        }

        await WriteFilesAsync(filePath, sb.ToString(), token);
    }

    #endregion

    #region Helpers

    private static string CreateExportFolder()
    {
        var folder = Path.Combine(
            AppContext.BaseDirectory,
            "exports");

        Directory.CreateDirectory(folder);

        return folder;
    }

    private static async Task WriteFilesAsync(
        string filePath,
        string content,
        CancellationToken token)
    {
        var encoding = new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: true);

        await File.WriteAllTextAsync(
            filePath,
            content,
            encoding,
            token);
    }

    private static string FormatDateTime(DateTime value)
    {
        return value.ToString(
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture);
    }

    private static string FormatNunber(double value)
    {
        return value.ToString(
            CultureInfo.InvariantCulture);
    }

    private static string CsvEscape(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        if (!value.Contains(',') &&
            !value.Contains('"') &&
            !value.Contains('\n') &&
            !value.Contains('\r'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    #endregion
}