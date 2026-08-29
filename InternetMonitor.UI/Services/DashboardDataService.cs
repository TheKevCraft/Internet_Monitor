using InternetMonitor.Core.Interfaces;
using InternetMonitor.UI.Interfaces;
using InternetMonitor.UI.Models;
using System.Threading;
using System.Threading.Tasks;

namespace InternetMonitor.UI.Services;

public class DashboardDataService : IDashboardDataService
{
    private readonly IDataService _dataService;
    private readonly IMonitorServiceController _serviceController;

    public DashboardDataService(
        IDataService dataService,
        IMonitorServiceController serviceController)
    {
        _dataService = dataService;
        _serviceController = serviceController;
    }

    public async Task<DashboardSnapshot> LoadAsync(
        CancellationToken token = default)
    {
        var workerStatusTask = _serviceController.GetStatusAsync(token);
        var connectivityTask = _dataService.GetLatestConnectivityAsync();
        var speedTestTask = _dataService.GetLatestSpeedTestAsync();

        await Task.WhenAll(
            workerStatusTask,
            connectivityTask,
            speedTestTask);

        return new DashboardSnapshot
        {
            WorkerStatus = await workerStatusTask,
            LatestConnectivity = await connectivityTask,
            LatestSpeedTest = await speedTestTask
        };
    }
}
