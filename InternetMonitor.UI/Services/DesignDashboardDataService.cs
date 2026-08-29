using InternetMonitor.Core.Models;
using InternetMonitor.UI.Interfaces;
using InternetMonitor.UI.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InternetMonitor.UI.Services;

public sealed class DesignDashboardDataService : IDashboardDataService
{
    public Task<DashboardSnapshot> LoadAsync(CancellationToken token = default)
    {
        var snapshot = new DashboardSnapshot
        {
            WorkerStatus = new MonitorServiceStatus
            {
                IsRunning = true,
                Status = "Läuft",
                Details = "Monitoring-Service ist aktiv"
            },
            LatestConnectivity = new ConnectivityResult
            {
                Timestamp = DateTime.Now,
                PingSuccess = true,
                PingMs = 24,
                DnsSuccess = true,
                HttpSuccess = true
            },
            LatestSpeedTest = new SpeedTestResult
            {
                Timestamp = DateTime.Now,
                PingMs = 24,
                Jitter = 2.1,
                DownloadMbps = 182.40,
                UploadMbps = 38.70,
                PacketLoss = 0,
                Success = true,
                PublicIp = "192.0.2.1",
                Isp = "Test ISP"
            }
        };

        return Task.FromResult(snapshot);
    }
}
