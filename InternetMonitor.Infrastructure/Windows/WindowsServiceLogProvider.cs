using InternetMonitor.Core.Interfaces;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Versioning;

namespace InternetMonitor.Infrastructure.Windows;

[SupportedOSPlatform("windows")]
internal class WindowsServiceLogProvider : IServiceLogProvider
{
    private const string LogName = "Application";
    private const string ProviderName = "InternetMonitor";

    public Task<IReadOnlyList<string>> GetLogsAsync(int lines = 50, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        var query = new EventLogQuery(
            LogName,
            PathType.LogName,
            $"*[System[Provider[@Name='{ProviderName}']]]")
        {
            ReverseDirection = true
        };

        using var reader = new EventLogReader(query);

        var logs = new List<string>(lines);

        for (var i = 0; i < lines; i++)
        {
            token.ThrowIfCancellationRequested();

            var entry = reader.ReadEvent();

            if (entry is null)
                break;

            using (entry)
            {
                logs.Add(
                    $"{entry.TimeCreated:yyyy-MM-dd HH:mm:ss} " +
                    $"{entry.LevelDisplayName}: " +
                    $"{entry.FormatDescription()}");
            }
        }

        return Task.FromResult<IReadOnlyList<string>>(logs);
    }
}
