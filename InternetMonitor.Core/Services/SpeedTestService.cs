using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using InternetMonitor.Core.Models.SpeedTest;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace InternetMonitor.Core.Services;

public class SpeedTestService : ISpeedTestService
{
    private readonly ILogger<SpeedTestService> _logger;
    private readonly ISpeedTestCliLocator _cliLocator;
    private static readonly JsonSerializerOptions JsonOptions = new() 
    {
        PropertyNameCaseInsensitive = true
    };

    private static ProcessStartInfo CreateProcess(string path)
    {
        return new ProcessStartInfo
            {
                FileName = path,
                Arguments = "--accept-license --accept-gdpr --format=json",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
    }

    public SpeedTestService(ILogger<SpeedTestService> logger, ISpeedTestCliLocator cliLocator)
    {
        _logger = logger;
        _cliLocator = cliLocator;
    }
    public async Task<SpeedTestResult> RunAsync(CancellationToken cancellationToken = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromMinutes(2));
        
        var result = new SpeedTestResult()
        {
            Timestamp = DateTime.UtcNow
        };

        var path = _cliLocator.GetPath();

        if (!File.Exists(path))
            throw new FileNotFoundException("Speedtest CLI missing", path);

        try
        {
            using var process = new Process
            {
                StartInfo = CreateProcess(path)
            };

            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();

            var errorTask = process.StandardError.ReadToEndAsync();    

            await process.WaitForExitAsync(timeout.Token);

            string output = await outputTask;
            string error = await errorTask;

            if (process.ExitCode != 0) 
            {
                result.Success = false;
                result.Error = error;
                return result;
            }

            var ooklaResult = JsonSerializer.Deserialize<OoklaResult>(output, JsonOptions);

            if (ooklaResult is null)
            {
                result.Success = false;
                result.Error = "Failed to deserialize Ookla result.";
                return result;
            }

            const double BitsPerByte = 8;
            const double BitsPerMegabit = 1_000_000;

            result.PingMs = ooklaResult.Ping.Latency;
            result.DownloadMbps = ooklaResult.Download.Bandwidth * BitsPerByte / BitsPerMegabit;
            result.UploadMbps = ooklaResult.Upload.Bandwidth * BitsPerByte / BitsPerMegabit;
            result.Success = true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Speedtest failed.");
            result.Success = false;
            result.Error = ex.Message;
        }

        return result;
    }
}
