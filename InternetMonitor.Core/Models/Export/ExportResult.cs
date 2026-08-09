namespace InternetMonitor.Core.Models;

public class ExportResult
{
    public ExportFormat Format { get; init; }

    public IReadOnlyList<string> Files { get; init; } = [];

    public DateTime GeneratedAt { get; init; }

    public bool Success => Files.Count > 0;
}
