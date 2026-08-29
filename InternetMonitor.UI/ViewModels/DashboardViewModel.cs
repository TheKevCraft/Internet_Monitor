using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.UI.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InternetMonitor.UI.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardDataService _dataService;
    private readonly IMonitorServiceController _serviceController;

    public DashboardViewModel(
        IDashboardDataService dataService,
        IMonitorServiceController serviceController)
    {
        _dataService = dataService;
        _serviceController = serviceController;
    }
    [ObservableProperty]
    private bool showServiceControls;

    [ObservableProperty]
    private bool isBusy;
    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    private bool isServiceInstalled;

    [ObservableProperty]
    private bool isWorkerRunning;

    [ObservableProperty]
    private string workerStatus = "Unbeaknnt";

    [ObservableProperty]
    private string workerDetails = "-";

    [ObservableProperty]
    private string connectivityStatus = "Unbekannt";

    [ObservableProperty]
    private string ping = "-";

    [ObservableProperty]
    private string download = "-";

    [ObservableProperty]
    private string upload = "-";

    [ObservableProperty]
    private string lastCheck = "-";

    [ObservableProperty]
    private string? errorMessage;

    public bool ShowInstallButton => !IsServiceInstalled;
    public bool ShowStartButton => IsServiceInstalled && !IsWorkerRunning;
    public bool ShowUninstallButton => IsServiceInstalled && !IsWorkerRunning;
    public bool ShowStopButton => IsServiceInstalled && IsWorkerRunning;
    public bool ShowRestartButton => IsServiceInstalled && IsWorkerRunning;

    partial void OnIsServiceInstalledChanged(bool value)
    {
        NotifyServiceActionPropertiesChanged();
    }

    partial void OnIsWorkerRunningChanged(bool value)
    {
        NotifyServiceActionPropertiesChanged();
    }

    private void NotifyServiceActionPropertiesChanged()
    {
        OnPropertyChanged(nameof(ShowInstallButton));
        OnPropertyChanged(nameof(ShowStartButton));
        OnPropertyChanged(nameof(ShowUninstallButton));
        OnPropertyChanged(nameof(ShowStopButton));
        OnPropertyChanged(nameof(ShowRestartButton));
    }

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }

    [RelayCommand]
    private async Task RefreshAsync(CancellationToken token = default)
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await RefreshCoreAsync(token);
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            ErrorMessage = $"Dashborad konnte nicht geladen werden: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task InstallWorkerAsync(CancellationToken token = default)
    {
        await ExecuteWorkerCommandAsync(() => _serviceController.InstallAsync(token));
    }

    [RelayCommand]
    private async Task UninstallWorkerAsync(CancellationToken token = default)
    {
        await ExecuteWorkerCommandAsync(() => _serviceController.UninstallAsync(token));
    }

    [RelayCommand]
    private async Task StartWorkerAsync(CancellationToken token = default)
    {
        await ExecuteWorkerCommandAsync(
            () => _serviceController.StartAsync(token));
    }

    [RelayCommand]
    private async Task StopWorkerAsync(CancellationToken token = default)
    {
        await ExecuteWorkerCommandAsync(
            () => _serviceController.StopAsync());
    }

    [RelayCommand]
    private async Task RestartWorkerAsync(CancellationToken token = default)
    {
        await ExecuteWorkerCommandAsync(
            () => _serviceController.RestartAsync());
    }

    private async Task RefreshCoreAsync(CancellationToken token = default)
    {
        var snapshot = await _dataService.LoadAsync();
        var serviceStatus = snapshot.WorkerStatus;
        IsServiceInstalled = serviceStatus.IsInstalled;
        IsWorkerRunning = serviceStatus.IsRunning;
        WorkerStatus = serviceStatus.Status;
        WorkerDetails = serviceStatus.Details ?? "-";
        if (snapshot.LatestConnectivity is not null)
        {
            var connectivity = snapshot.LatestConnectivity;
            ConnectivityStatus = 
                connectivity.IsOnline
                    ? "Online"
                    : "Offline";

            Ping = connectivity.PingMs is long pingMs
                ? $"{pingMs} ms"
                : "-";

            LastCheck = connectivity.Timestamp
                .ToLocalTime()
                .ToString("dd.MM.yyyy, HH:mm:ss");
        }
        else
        {
            ConnectivityStatus = "Keine Daten";
            Ping = "-";
        }

        if (snapshot.LatestSpeedTest is not null)
        {
            Download = $"{snapshot.LatestSpeedTest.DownloadMbps:F2} Mbps";
            Upload = $"{snapshot.LatestSpeedTest.UploadMbps:F2} Mbps";
        }
        else
        {
            Download = "-";
            Upload = "-";
        }
    }

    private async Task ExecuteWorkerCommandAsync(Func<Task> action)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = null;
            await action();
            await RefreshCoreAsync();
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            ErrorMessage = $"Worker-Aktion fehlgeschlagen: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
