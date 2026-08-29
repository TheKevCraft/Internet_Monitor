namespace InternetMonitor.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public DashboardViewModel Dashboard { get; }
    public MainViewModel(DashboardViewModel dashboard)
    {
        Dashboard = dashboard;
    }
}
