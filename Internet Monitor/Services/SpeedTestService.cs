using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Internet_Monitor.Models.SpeedTest;
using System.Diagnostics;
using System.Text.Json;

namespace Internet_Monitor.Services;

public class SpeedTestService : ISpeedTestService
{
    public async Task<SpeedTestResult> RunAsync(CancellationToken cancellationToken = default)
    {
        var result = new SpeedTestResult()
        {
            Timestamp = DateTime.UtcNow
        };

        var path = SpeedTestCliLocator.GetPath();

        if (!File.Exists(path))
            throw new FileNotFoundException("Speedtest CLI missing", path);

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = path,
                Arguments = "--format=json",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process
            {
                StartInfo = psi
            };

            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();

            /*Console.WriteLine("=== SPEEDTEST RAW OUTPUT ===");
            Console.WriteLine(output);
            Console.WriteLine("============================");*/

            string error = await process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync(cancellationToken);

            /*Console.WriteLine($"ExitCode: {process.ExitCode}");
            Console.WriteLine($"Error: {error}");*/

            if (process.ExitCode != 0) 
            {
                result.Success = false;
                result.Error = error;
                return result;
            }

            var ooklaResult = JsonSerializer.Deserialize<OoklaResult>(output);

            if (ooklaResult is null)
            {
                result.Success = false;
                result.Error = "Failed to deserialize Ookla result.";
                return result;
            }

            result.PingMs = ooklaResult.Ping.Latency;
            result.DownloadMbps = (ooklaResult.Download.Bandwidth * 8) / 1_000_000;
            result.UploadMbps = (ooklaResult.Upload.Bandwidth * 8) / 1_000_000;
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = ex.Message;
        }

        return result;
    }
}
