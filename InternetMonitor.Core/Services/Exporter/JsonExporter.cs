using System.Text.Json;
using InternetMonitor.Core.Configs.Options;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Services.Exporter;

internal class JsonExporter : IExporter
{
    public ExportFormat Format => ExportFormat.Json;

    private static string _basePath = "";

    private static readonly JsonSerializerOptions Options = new ()
    {
        WriteIndented = true
    };

    public JsonExporter(MonitorOptions options)
    {
        _basePath = ApplicationPaths.Resolve(options.Export.Directory);
    }

    public async Task<ExportResult> ExportAsync(InternetExportData data, CancellationToken token = default)
    {
        var path = CreatePath("InternetReport", "json");

        var json = JsonSerializer.Serialize(data, Options);

        await File.WriteAllTextAsync(path, json, token);

        return new ExportResult
        {
            Format = Format,
            GeneratedAt = data.GeneratedAt,
            Files = [path]
        };
    }

    private static string CreatePath(string name, string extension)
    {
        Directory.CreateDirectory(_basePath);

        return Path.Combine(
            _basePath,
            $"{name}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.{extension}");
    }
}