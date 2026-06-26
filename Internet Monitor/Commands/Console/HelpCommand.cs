using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace Internet_Monitor.Commands.Console;

public class HelpCommand : IConsoleCommand
{
    public string Name => "help";
    public string Usage => "";
    public string Description => "Show all commands";
    public IEnumerable<string> Aliases => [];

    public Task ExecuteAsync(CommandContext context)
    {
        var commands = context.Services.GetServices<IConsoleCommand>();

        var table = new Table();
        table.AddColumn("Command");
        table.AddColumn("Usage");
        table.AddColumn("Description");

        foreach (var cmd in commands
                .DistinctBy(c => c.Name)
                .OrderBy(c => c.Name))
        {
            table.AddRow(
                cmd.Name,
                cmd.Usage,
                cmd.Description);
        }

        AnsiConsole.Write(table);

        return Task.CompletedTask;
    } 
}
