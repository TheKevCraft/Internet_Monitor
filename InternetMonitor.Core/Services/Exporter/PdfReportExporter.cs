using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using QuestPDF.Fluent;

namespace InternetMonitor.Core.Services.Exporter;

internal class PdfReportExporter : IExporter
{
    public ExportFormat Format => ExportFormat.Pdf;

    public Task<string> ExportAsync(InternetExportData data, CancellationToken token = default)
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "reports");

        Directory.CreateDirectory(folder);

        var path = Path.Combine(folder, $"InternetReport_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.pdf");

        var report = data.Summary;

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
                            $"Generated: {report.GeneratedAt}");

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

        return Task.FromResult(path);
    }
}
