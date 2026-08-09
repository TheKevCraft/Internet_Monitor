using InternetMonitor.Core.Config;
using InternetMonitor.Core.Data;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Services;
using InternetMonitor.Core.Services.Exporter;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInternetMonitorCore(this IServiceCollection services)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        // Test Services
        services.AddHttpClient<IConnectivityService, ConnectivityService>();
        services.AddSingleton<ISpeedTestService, SpeedTestService>();

        // Database
        services.AddSingleton<SqliteConnectionFactory>();
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
        services.AddSingleton<IDataService, DataService>();

        // Export & Report Services
        services.AddSingleton<IExportService, ExportService>();
        services.AddSingleton<IReportService, ReportService>();

        // Exporter
        services.AddSingleton<IExporter, PdfReportExporter>();
        services.AddSingleton<IExporter, JsonExporter>();
        services.AddSingleton<IExporter, CsvExporter>();

        // Monitor
        services.AddSingleton<IMonitorService, MonitorService>();
        services.AddSingleton<IExecutionPolicyService, ExecutionPolicyService>();
        //services.AddHostedService<MonitorHostedService>();
        
        // Config
        services.AddSingleton<DatabaseOptions>();

        // Others
        services.AddSingleton<ISpeedTestCliLocator, SpeedTestCliLocator>();

        return services;
    }
}