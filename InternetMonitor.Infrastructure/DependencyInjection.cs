using InternetMonitor.Core.Interfaces;
using InternetMonitor.Infrastructure.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace InternetMonitor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInternetMonitorInfrastructure(
        this IServiceCollection services)
    {
        if (OperatingSystem.IsWindows())
        {
            services.AddSingleton<IMonitorServiceController, WindowsMonitorServiceController>();
            services.AddSingleton<IServiceLogProvider, WindowsServiceLogProvider>();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }

        return services;
    }
}
