using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using QuestPDF.Fluent;

namespace Internet_Monitor.Services;

public class PdfReportExporter : IPdfReportExporter
{
    public async Task<string> ExportAsync(InternetReport report)
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "reports");

        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, $"InternetReport_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("Internet Monitor Report")
                    .FontSize(20);

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text(
                            $"Ganerated: {report.GeneratedAt}");

                        column.Item().Text(
                            $"Uptime: {report.UptimePercent:F2}%");

                        column.Item().Text(
                            $"Average Ping: {report.AveragePing:F1} ms");

                        column.Item().Text(
                            $"Average Download: {report.AverageDownload:F1} Mbps");

                        column.Item().Text(
                            $"Average Upload: {report.AverageUpload:F1} Mbps");

                        column.Item().Text(
                            $"Offline Events: {report.OfflineEvents}");

                        column.Item().Text(
                            $"Speedtests: {report.SpeedTestCount}");
                    });
            });
        })
        .GeneratePdf(path);

        return path;
    }
}
