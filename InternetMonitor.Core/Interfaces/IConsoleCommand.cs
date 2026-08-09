using InternetMonitor.Core.Models;

namespace InternetMonitor.Core.Interfaces;

public interface IConsoleCommand
{
    string Name { get; }
    string Description { get; }
    string Usage { get; }
    IEnumerable<string> Aliases { get; }

    Task ExecuteAsync(CommandContext context);
}
