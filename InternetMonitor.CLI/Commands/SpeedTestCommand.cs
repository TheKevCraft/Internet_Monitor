using System.CommandLine;
using InternetMonitor.Core.Interfaces;

namespace InternetMonitor.Cli.Commands;

public sealed class SpeedTestCommand : ICommandModule
{
    private readonly ISpeedTestService _speedTest;

    public SpeedTestCommand(ISpeedTestService speedTest)
    {
        _speedTest = speedTest;
    }

    public Command Create()
    {
        var command = new Command("speedtest", "Run a speedtest.");

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var result = await _speedTest.RunAsync(cancellationToken);

            if (!result.Success)
            {
                ConsoleOutput.PrintError(result.Error!);
                return;
            }

            ConsoleOutput.PrintSpeedTest(result);
        });

        return command;
    }
}