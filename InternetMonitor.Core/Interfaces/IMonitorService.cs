namespace InternetMonitor.Core.Interfaces;

public interface IMonitorService
{
    Task RunAsync(CancellationToken token);
}