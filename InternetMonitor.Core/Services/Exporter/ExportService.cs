using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using Microsoft.Extensions.Logging;

namespace InternetMonitor.Core.Services;

public class ExportService : IExportService
{
    private readonly IDataService _dataService;
    private readonly IReportService _reportService;
    private readonly IEnumerable<IExporter> _exporters;
    private readonly ILogger<ExportService> _logger;

    public ExportService(
        IDataService dataService, 
        IReportService reportService, 
        IEnumerable<IExporter> exporters, 
        ILogger<ExportService> logger)
    {
        _dataService = dataService;
        _reportService = reportService;
        _exporters = exporters;
        _logger = logger;
    }

    public async Task<string> ExportAsync(
        ExportFormat format,
        int limit,  
        CancellationToken token = default)
    {

        _logger.LogInformation("Starting {Format} export.", format);

        var data = new InternetExportData
        {
            GeneratedAt = DateTime.UtcNow,

            Summary = await _reportService.GenerateReportAsync(limit),

            Connectivity = await _dataService.GetConnectivityHistoryAsync(limit, token),

            SpeedTests = await _dataService.GetSpeedTestHistoryAsync(limit, token)
        };
        
        var exporter = _exporters.FirstOrDefault(x => x.Format == format);

        if (exporter == null)
            throw new NotSupportedException(
                $"Export format {format} not supported");

        try
        {
            return await exporter.ExportAsync(data, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export as {Format}.", format);
        
            throw;
        }
    }
}
