using InternetMonitor.Core.Configs;
using InternetMonitor.Core.Data;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Services;
using InternetMonitor.Core.Services.Exporter;
using Microsoft.Extensions.DependencyInjection;

namespace InternetMonitor.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInternetMonitorCore(this IServiceCollection services)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        // Setup Directorys
        ApplicationPaths.Initialize();

        // Config
        var configPath = Path.Combine(AppContext.BaseDirectory,"Configs", "monitor.toml");

        var config = TomlConfigLoader.Load(configPath);

        services.AddSingleton(config);

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

        // Others
        services.AddSingleton<ISpeedTestCliLocator, SpeedTestCliLocator>();

        return services;
    }
}