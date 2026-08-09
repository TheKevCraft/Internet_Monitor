using System.CommandLine;
using InternetMonitor.Core.Interfaces;

namespace InternetMonitor.Cli.Commands;

public sealed class ConnectivityCommand : ICommandModule
{
    private readonly IConnectivityService _connectivity;

    public ConnectivityCommand(IConnectivityService connectivity)
    {
        _connectivity = connectivity;
    }

    public Command Create()
    {
        var command = new Command("connectivity", "Run a connectivity test.");

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var result = await _connectivity.CheckAsync(cancellationToken);

            ConsoleOutput.PrintConnectivity(result);
        });

        return command;
    }
}