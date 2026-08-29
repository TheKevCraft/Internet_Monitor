using InternetMonitor.UI.Interfaces;
using InternetMonitor.UI.Services;
using InternetMonitor.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InternetMonitor.UI;

public static class DependencyInjection
{
    public static IServiceCollection AddInternetMonitorUi(
        this IServiceCollection services)
    {
        services.AddSingleton<IDashboardDataService, DashboardDataService>();

        services.AddTransient<DashboardViewModel>();
        services.AddTransient<MainViewModel>();

        return services;
    }
}
