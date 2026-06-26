using Internet_Monitor.Models;

namespace Internet_Monitor.Interfaces;

public interface IConsoleCommand
{
    string Name { get; }
    string Description { get; }
    string Usage { get; }
    IEnumerable<string> Aliases { get; }

    Task ExecuteAsync(CommandContext context);
}
