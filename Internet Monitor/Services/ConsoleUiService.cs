using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;

namespace Internet_Monitor.Services;

public class ConsoleUiService
{
    private readonly IServiceProvider _provider;

    private readonly Dictionary<string, IConsoleCommand> _commands;
    private bool _running = true;

    public ConsoleUiService(IServiceProvider provider, IEnumerable<IConsoleCommand> commands)
    {
        _provider = provider;
        _commands = new(StringComparer.OrdinalIgnoreCase);

        foreach (var command in commands)
        {
            Register(command);
        }
    }
    

    public async Task RunAsync(CancellationToken token)
    {
        ShowBanner();

        while (_running && !token.IsCancellationRequested)
        {
            Console.Write("NetMonitor> ");

            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var split = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var commandName = split[0];

            if (!_commands.TryGetValue(commandName, out var command))
            {
                Console.WriteLine($"Unknown command: {commandName}");
                continue;
            }

            var context = new CommandContext
            {
                Services = _provider,
                CancellationToken = token,
                RawInput = input,
                Command = commandName,
                Arguments = split.Skip(1).ToArray()
            };

            try
            {
                await command.ExecuteAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }


    // | \\

    private static void ShowBanner()
    {
        Console.WriteLine("=================================");
        Console.WriteLine("        Internet Monitor");
        Console.WriteLine("=================================");
        Console.WriteLine("Type 'help' for commands");
        Console.WriteLine("");
    }

    // Helpers
    private void Register(IConsoleCommand command)
    {
        _commands[command.Name] = command;

        foreach (var alias in command.Aliases)
        {
            _commands[alias] = command;
        }
    }
}
