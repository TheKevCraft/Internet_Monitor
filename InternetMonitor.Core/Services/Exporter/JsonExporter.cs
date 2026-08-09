using System.Text.Json;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Services.Exporter;

internal class JsonExporter : IExporter
{
    public ExportFormat Format => ExportFormat.Json;

    private static readonly JsonSerializerOptions Options = new ()
    {
        WriteIndented = true
    };

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
        var folder = Path.Combine(AppContext.BaseDirectory, "exports");

        Directory.CreateDirectory(folder);

        return Path.Combine(
            folder,
            $"{name}_{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}.{extension}");
    }
}