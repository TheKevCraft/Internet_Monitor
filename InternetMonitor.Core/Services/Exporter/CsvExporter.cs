using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using System.Globalization;
using System.Text;

namespace InternetMonitor.Core.Services.Exporter;

internal class CsvExporter : IExporter
{
    public ExportFormat Format => ExportFormat.Csv;

    public async Task<string> ExportAsync(InternetExportData data, CancellationToken token = default)
    {
        var path = CreatePath("InternetReport", "csv");

        var rows = data.SpeedTests.Select(x =>
            string.Join(",",
            x.Timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            x.PingMs.ToString(CultureInfo.InvariantCulture) ?? "",
            x.DownloadMbps.ToString(CultureInfo.InvariantCulture),
            x.UploadMbps.ToString(CultureInfo.InvariantCulture),
            x.Jitter.ToString(CultureInfo.InvariantCulture),
            x.PacketLoss.ToString(CultureInfo.InvariantCulture),
            CsvEscape(x.Isp),
            CsvEscape(x.ServerName)));

        await ExportCsvAsync(
            path,
            "Timestamp,PingMs,Download,Upload,Jitter,PacketLoss,ISP,ServerName",
            rows,
            token);

        return path;
    }

    private static string CreatePath(string name, string extension)
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "exports");

        Directory.CreateDirectory(folder);

        return Path.Combine(
            folder,
            $"{name}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.{extension}");
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

    private static async Task ExportCsvAsync(string filePath, string header, IEnumerable<string> rows, CancellationToken token = default)
    {
        var sb = new StringBuilder();

        sb.AppendLine(header);

        foreach (var row in rows)
            sb.AppendLine(row);

        var endcoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

        await File.WriteAllTextAsync(filePath, sb.ToString(), endcoding, token);
    }
}