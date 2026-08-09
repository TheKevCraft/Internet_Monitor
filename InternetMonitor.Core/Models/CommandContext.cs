namespace InternetMonitor.Core.Models;

public sealed class CommandContext
{
    public required IServiceProvider Services { get; init; }
    public required CancellationToken CancellationToken { get; init; }
    public required string RawInput { get; init; }
    public required string Command { get; init; }
    public required string[] Arguments { get; init; }
}
