using System.Threading;
using System.Threading.Tasks;
using InternetMonitor.UI.Models;

namespace InternetMonitor.UI.Interfaces;

public interface IDashboardDataService
{
    Task<DashboardSnapshot> LoadAsync(CancellationToken token = default);
}
