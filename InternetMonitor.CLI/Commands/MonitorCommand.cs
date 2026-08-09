using System.CommandLine;
using InternetMonitor.Core.Interfaces;

namespace InternetMonitor.Cli.Commands;

public sealed class MonitorCommand : ICommandModule
{
    private readonly IMonitorService _monitorService;

    public MonitorCommand(IMonitorService monitorService)
    {
        _monitorService = monitorService;
    }

    public Command Create()
    {
        var command = new Command("monitor", "Starts internet monitoring.");

        command.SetAction(async (_, token) =>
        {
            await _monitorService.RunAsync(token);
        });

        return command;
    }
}