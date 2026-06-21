using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using System.Text;
using System.Text.Json;

namespace Internet_Monitor.Services;

public class ExportService : IExportService
{
    private readonly IDataService _dataService;

    public ExportService(IDataService dataService)
    {
        _dataService = dataService;
    }

    public async Task<string> ExportSpeedTestsAsync(ExportFormat format, int limit)
    {
        var data = await _dataService.GetSpeedTestHistoryAsync(limit);

        var filePath = GetFilePath("speedtests",limit, format);

        switch (format)
        {
            case ExportFormat.Csv:
                var sb = new StringBuilder();

                sb.AppendLine("Timestamp,Ping,Download,Upload,Jitter,PacketLoss,ISP,Server");

                foreach (var item in data)
                {
                    sb.AppendLine(
                        $"{item.Timestamp.ToString("yyy-MM-dd HH:mm:ss")}," +
                        $"{item.PingMs}," +
                        $"{item.DownloadMbps}," +
                        $"{item.UploadMbps}," +
                        $"{item.Jitter}," +
                        $"{item.PacketLoss}," +
                        $"{CsvEscape(item.Isp)}," +
                        $"{CsvEscape(item.ServerName)}");
                }

                await File.WriteAllTextAsync(filePath, sb.ToString());
                break;
            case ExportFormat.Json:
                await ExportJsonAsync(filePath, data);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }

    return filePath;
    }

    public async Task<string> ExportConnectivityAsync(ExportFormat format, int limit)
    {
        var data = await _dataService.GetConnectivityHistoryAsync(limit);

        var filePath = GetFilePath("connectivity", limit, format);

        switch (format)
        {
            case ExportFormat.Csv:
                var sb = new StringBuilder();

                sb.AppendLine("Timestamp,Online,PingSuccess,PingMs,Dns,Http");

                foreach (var item in data)
                {
                    sb.AppendLine(
                        $"{item.Timestamp.ToString("yyy-MM-dd HH:mm:ss")}," +
                        $"{item.IsOnline}," +
                        $"{item.PingSuccess}," +
                        $"{item.PingMs}," +
                        $"{item.DnsSuccess}," +
                        $"{item.HttpSuccess}");
                }

                await File.WriteAllTextAsync(filePath, sb.ToString());
                break;
            case ExportFormat.Json:
                await ExportJsonAsync(filePath, data);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }

        return filePath;
    }

    private string GetFilePath(string name, int limit, ExportFormat format)
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "exports");
        Directory.CreateDirectory(folder);

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        return Path.Combine(folder, $"{name}_{limit}_{timestamp}.{format.ToString().ToLower()}");
    }

    private static string CsvEscape(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private static async Task ExportJsonAsync<T>(string filePath, IEnumerable<T> data)
    {
        var json = JsonSerializer.Serialize(
            data,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        await File.AppendAllTextAsync(filePath, json);
    }
}
