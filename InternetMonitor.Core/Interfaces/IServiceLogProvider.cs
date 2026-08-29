namespace InternetMonitor.Core.Interfaces;

public interface IServiceLogProvider
{
    Task<IReadOnlyList<string>> GetLogsAsync(int lines = 50, CancellationToken token = default);
}
