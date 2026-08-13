using InternetMonitor.CLI.Services;
using InternetMonitor.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace InternetMonitor.CLI;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInternetMonitorCli(this IServiceCollection services)
    {
        if (OperatingSystem.IsWindows())
        {
            services.AddSingleton<IMonitorServiceController, WindowsMonitorServiceController>();
        }

        return services;
    }
}
