using InternetMonitor.Core.Constants;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.ServiceProcess;

namespace InternetMonitor.Infrastructure.Windows;

[SupportedOSPlatform("windows")]
internal class WindowsMonitorServiceController : IMonitorServiceController
{
    private readonly IServiceLogProvider _logProvider;

    public WindowsMonitorServiceController(IServiceLogProvider logProvider)
    {
        _logProvider = logProvider;
    }



    #region install

    public async Task InstallAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException(
                "The Windows service controller can only be used on Windows.");
        }

        var executionPath = GetServiceExecutablePath();

        if (!File.Exists(executionPath))
        {
            throw new FileNotFoundException(
                "The Internet Monitor service executable was not found.",
                executionPath);
        }

        if (ServiceExists())
        {
            throw new InvalidOperationException(
                $"The Windows service '{MonitorServiceConstants.ServiceName}' is already installed.");
        }

        var arguments = 
            $"create \"{MonitorServiceConstants.ServiceName}\" "+
            $"binPath= \"{executionPath}\" " +
            "start= demand " +
            $"DisplayName= \"{MonitorServiceConstants.DisplayName}\"";

        await RunScAsync(arguments, token);

        await RunScAsync(
            $"description \"{MonitorServiceConstants.ServiceName}\" \"{MonitorServiceConstants.Description}\"",
            token);
    }

    public async Task UninstallAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException(
                "The Windows service controller can only be used on Windows.");
        }

        if (!ServiceExists())
        {
            return;
        }

        using var service = new ServiceController(MonitorServiceConstants.ServiceName);

        service.Refresh();

        if (service.Status != ServiceControllerStatus.Stopped)
        {
            service.Stop();

            await WaitForStatusAsync(
                service,
                ServiceControllerStatus.Stopped,
                TimeSpan.FromSeconds(30),
                token);
        }

        await RunScAsync(
            $"delete \"{MonitorServiceConstants.ServiceName}\"",
            token);
    }

    #endregion

    #region Controls
    
    public Task StartAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        using var service = new ServiceController(MonitorServiceConstants.ServiceName);

        service.Refresh();

        if (service.Status == ServiceControllerStatus.Running)
            return Task.CompletedTask;

        service.Start();
        service.WaitForStatus(
            ServiceControllerStatus.Running,
            TimeSpan.FromSeconds(30));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        using var service = new ServiceController(MonitorServiceConstants.ServiceName);

        service.Refresh();

        if (service.Status == ServiceControllerStatus.Stopped)
            return Task.CompletedTask;

        service.Stop();
        service.WaitForStatus(
            ServiceControllerStatus.Stopped,
            TimeSpan.FromSeconds(30));

        return Task.CompletedTask;
    }

    public async Task RestartAsync(CancellationToken token = default)
    {
        await StopAsync(token);
        await StartAsync(token);
    }

    public Task<MonitorServiceStatus> GetStatusAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (!ServiceExists())
        {
            return Task.FromResult(
                new MonitorServiceStatus
                {
                    IsInstalled = false,
                    IsRunning = false,
                    Status = "Not installed",
                    Details = "The Windows-Service was not found."
                });
        }

        using var service = new ServiceController(MonitorServiceConstants.ServiceName);

        service.Refresh();

        return Task.FromResult(
            new MonitorServiceStatus
            {
                IsInstalled = true,
                IsRunning = service.Status == ServiceControllerStatus.Running,
                Status = service.Status.ToString(),
                Details = $"Windows Service: {MonitorServiceConstants.ServiceName}"
            });
    }

    public Task<IReadOnlyList<string>> GetLogsAsync(
        int lines = 50,
        CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        return _logProvider.GetLogsAsync(lines, token);
    }

    #endregion

    #region Helpers

    private static string GetServiceExecutablePath()
    {
        var applicationDirectory = new DirectoryInfo(AppContext.BaseDirectory).Parent
            ?? throw new InvalidOperationException(
                "The application directory could not be determined.");

        var path = Path.Combine(
            applicationDirectory.FullName,
            "Service",
            MonitorServiceConstants.ExecutableName);

        /*var path = Path.Combine(
            AppContext.BaseDirectory,
            "Service",
            MonitorServiceConstants.ExecutableName);*/

        return Path.GetFullPath(path);
    }

    private static bool ServiceExists(/*string serviceName*/)
    {
        return ServiceController
            .GetServices()
            .Any(service =>
                service.ServiceName.Equals(
                    MonitorServiceConstants.ServiceName,
                    StringComparison.OrdinalIgnoreCase));
        /*try
        {
            using var service = new ServiceController(serviceName);

            service.Refresh();

            _ = service.Status;

            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }*/
    }

    private static async Task RunScAsync(string args, CancellationToken token)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "sc.exe",
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = 
            Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                "Could not start sc.exe.");

        var outputTask = process.StandardOutput.ReadToEndAsync(token);
        var errorTask = process.StandardError.ReadToEndAsync(token);

        await process.WaitForExitAsync(token);

        var output = await outputTask;
        var error = await errorTask;

        if (process.ExitCode != 0)
        {
            if (process.ExitCode == 5)
            {
                throw new InvalidOperationException(
                    "Access denied. Installing or modifying a windows service requires administrator privileges.");
            }

            throw new InvalidOperationException(
                "sc.exe failed with exit code " +
                $"{process.ExitCode}: " +
                $"{error.Trim()}");
        }
    }

    private static async Task WaitForStatusAsync(
        ServiceController service,
        ServiceControllerStatus desiredStatus,
        TimeSpan timeout,
        CancellationToken token)
    {
        var start = DateTime.UtcNow;

        while (service.Status != desiredStatus)
        {
            token.ThrowIfCancellationRequested();

            if (DateTime.UtcNow - start > timeout)
            {
                throw new System.TimeoutException(
                    $"The service '{service.ServiceName}' " +
                    $"did not reach status '{desiredStatus}'.");
            }

            await Task.Delay(
                TimeSpan.FromMilliseconds(250),
                token);

            service.Refresh();
        }
    }

    #endregion
}
