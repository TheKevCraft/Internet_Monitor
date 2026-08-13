using System.CommandLine;
using InternetMonitor.Core.Interfaces;

namespace InternetMonitor.Cli.Commands;

public sealed class MonitorCommand : ICommandModule
{
    private readonly IMonitorServiceController _monitorService;

    public MonitorCommand(IMonitorServiceController monitorService)
    {
        _monitorService = monitorService;
    }

    public Command Create()
    {
        var command = new Command(
            "monitor",
            "Control the Internet Monitor service.");
        


        return command;
    }

    private Command CreateStartCommand()
    {
        var command = new Command(
            "start",
            "Start the Internet Monitor service.");

        command.SetAction(async (parseResult, token) =>
        {
            await _monitorService.StartAsync(token);

            Console.WriteLine("Internet Monitor service started.");
        });

        return command;
    }

    private Command CreateStopCommand()
    {
        var command = new Command(
            "stop",
            "Stop the Internet Monitor service.");

        command.SetAction(async (parseResult, token) =>
        {
            await _monitorService.StopAsync(token);

            Console.WriteLine("Internet Monitor service stopped.");
        });

        return command;
    }

    private Command CreateRestartCommand()
    {
        var command = new Command(
            "restart",
            "Restart the Internet Monitor service.");

        command.SetAction(async (parseResult, token) =>
        {
            await _monitorService.RestartAsync(token);

            Console.WriteLine("Internet Monitor service restarted.");
        });

        return command;
    }

    private Command CreateStatusCommand()
    {
        var command = new Command(
            "status",
            "Show the status of the Internet Monitor service.");

        command.SetAction(async (parseResult, token) =>
        {
            var status = await _monitorService.GetStatusAsync(token);

            Console.WriteLine($"Status: {status.Status}");
            Console.WriteLine($"Running: {status.IsRunning}");

            if (!string.IsNullOrWhiteSpace(status.Details))
            {
                Console.WriteLine($"Details: {status.Details}");
            }
        });

        return command;
    }

    private Command CreateLogsCommand()
    {
        var linesOption = new Option<int>("--lines")
        {
            Description = "Number of log lines to display.",
            DefaultValueFactory = _ => 50
        };

        var command = new Command(
            "logs",
            "Show Internet Monitor service logs.");

        command.Options.Add(linesOption);

        command.SetAction(async (parseResult, token) =>
        {
            var lines = parseResult.GetValue(linesOption);

            var logs = await _monitorService.GetLogsAsync(lines, token);

            foreach (var line in logs)
            {
                Console.WriteLine(line);
            }
        });

        return command;
    }
}