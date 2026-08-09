using InternetMonitor.Core.Config;
using InternetMonitor.Core.Interfaces;
using InternetMonitor.Core.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace InternetMonitor.Core.Data;

public class DataService : IDataService
{
    private readonly ILogger<DataService> _logger;
    private readonly SqliteConnectionFactory _factory;

    public DataService(DatabaseOptions options, ILogger<DataService> logger, SqliteConnectionFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    #region Saves
    public async Task SaveConnectivityAsync(ConnectivityResult result, CancellationToken token = default)
    {
        await using var connection = await _factory.OpenConnectionAsync();

        var command = connection.CreateCommand();

        command.CommandText =
        """
    INSERT INTO ConnectivityResults
    (
        Timestamp,
        IsOnline,
        PingSuccess,
        PingMs,
        DnsSuccess,
        HttpSuccess
    )
    VALUES
    (
        $timestamp,
        $online,
        $pingSuccess,
        $pingMs,
        $dnsSuccess,
        $httpSuccess
    );
    """;

        command.Parameters.AddWithValue(
            "$timestamp",
            new DateTimeOffset(result.Timestamp).ToUnixTimeSeconds());

        command.Parameters.AddWithValue(
            "$online",
            result.IsOnline);

        command.Parameters.AddWithValue(
            "$pingSuccess",
            result.PingSuccess);

        command.Parameters.AddWithValue(
            "$pingMs",
            (object?)result.PingMs ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "$dnsSuccess",
            result.DnsSuccess);

        command.Parameters.AddWithValue(
            "$httpSuccess",
            result.HttpSuccess);

        try
        {
            await command.ExecuteNonQueryAsync(token);
        }
        catch(Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed saving connectivity result");

            throw;
        }
    }

    public async Task SaveSpeedTestAsync(SpeedTestResult result, CancellationToken token = default)
    {
        await using var connection = await _factory.OpenConnectionAsync();

        var command = connection.CreateCommand();

        command.CommandText =
        """
    INSERT INTO SpeedTests
    (
        Timestamp,
        PingMs,
        Jitter,
        DownloadMbps,
        UploadMbps,
        PacketLoss,
        PublicIp,
        Isp,
        ServerName,
        ServerLocation
    )
    VALUES
    (
        $timestamp,
        $ping,
        $jitter,
        $download,
        $upload,
        $packetLoss,
        $publicIp,
        $isp,
        $serverName,
        $serverLocation
    );
    """;

        command.Parameters.AddWithValue("$timestamp", new DateTimeOffset(result.Timestamp).ToUnixTimeSeconds());
        command.Parameters.AddWithValue("$ping", result.PingMs);
        command.Parameters.AddWithValue("$jitter", result.Jitter);
        command.Parameters.AddWithValue("$download", result.DownloadMbps);
        command.Parameters.AddWithValue("$upload", result.UploadMbps);
        command.Parameters.AddWithValue("$packetLoss", result.PacketLoss);
        command.Parameters.AddWithValue("$publicIp", result.PublicIp ?? "");
        command.Parameters.AddWithValue("$isp", result.Isp ?? "");
        command.Parameters.AddWithValue("$serverName", result.ServerName ?? "");
        command.Parameters.AddWithValue("$serverLocation", result.ServerLocation ?? "");

        try
        {
            await command.ExecuteNonQueryAsync(token);
        }
        catch(Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed saving speedtest result");

            throw;
        }
    }

    #endregion

    #region Get`s
    public async Task<List<ConnectivityResult>> GetConnectivityHistoryAsync(int limit = 100, CancellationToken token = default)
    {
        limit = Math.Clamp(limit, 1, 1000);

        var results = new List<ConnectivityResult>();

        await using var connection = await _factory.OpenConnectionAsync();

        var command = connection.CreateCommand();

        command.CommandText =
        """
        SELECT *
        FROM ConnectivityResults
        ORDER BY Timestamp DESC
        LIMIT $limit;
        """;

        command.Parameters.AddWithValue("$limit", limit);

        await using var reader = 
            await command.ExecuteReaderAsync(token);

        while (await reader.ReadAsync())
        {
            results.Add(new ConnectivityResult
            {
                Timestamp = DateTimeOffset
                    .FromUnixTimeSeconds(Convert.ToInt64(reader["Timestamp"]))
                    .DateTime,
                PingSuccess = Convert.ToBoolean(reader["PingSuccess"]),
                PingMs = reader["PingMs"] == DBNull.Value
                ? null
                : Convert.ToInt64(reader["PingMs"]),
                DnsSuccess = Convert.ToBoolean(reader["DnsSuccess"]),
                HttpSuccess = Convert.ToBoolean(reader["HttpSuccess"])
            });
        }

        return results;
    }

    public async Task<List<SpeedTestResult>> GetSpeedTestHistoryAsync(int limit = 100, CancellationToken token = default)
    {
        limit = Math.Clamp(limit, 1, 1000);

        var results = new List<SpeedTestResult>();

        await using var connection = await _factory.OpenConnectionAsync();

        var command = connection.CreateCommand();

        command.CommandText =
        """
    SELECT *
    FROM SpeedTests
    ORDER BY Timestamp DESC
    LIMIT $limit;
    """;

        command.Parameters.AddWithValue("$limit", limit);

        await using var reader =
            await command.ExecuteReaderAsync(token);

        while (await reader.ReadAsync())
        {
            results.Add(new SpeedTestResult
            {
                Timestamp = DateTimeOffset
                    .FromUnixTimeSeconds(Convert.ToInt64(reader["Timestamp"]))
                    .DateTime,
                PingMs = Convert.ToDouble(reader["PingMs"]),
                Jitter = Convert.ToDouble(reader["Jitter"]),
                DownloadMbps = Convert.ToDouble(reader["DownloadMbps"]),
                UploadMbps = Convert.ToDouble(reader["UploadMbps"]),
                PacketLoss = Convert.ToDouble(reader["PacketLoss"]),
                PublicIp = reader["PublicIp"]?.ToString(),
                Isp = reader["Isp"]?.ToString(),
                ServerName = reader["ServerName"]?.ToString(),
                ServerLocation = reader["ServerLocation"]?.ToString(),
                Success = true
            });
        }

        return results;
    }

    public async Task<ConnectivityResult?> GetLatestConnectivityAsync()
    {
        await using var connection = await _factory.OpenConnectionAsync();

        var command = connection.CreateCommand();

        command.CommandText =
        """
    SELECT *
    FROM ConnectivityResults
    ORDER BY Timestamp DESC
    LIMIT 1;
    """;

        await using var reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new ConnectivityResult
        {
            Timestamp = DateTimeOffset
                .FromUnixTimeSeconds(Convert.ToInt64(reader["Timestamp"]))
                .DateTime,
            PingSuccess = Convert.ToBoolean(reader["PingSuccess"]),
            PingMs = reader["PingMs"] == DBNull.Value
                ? null
                : Convert.ToInt64(reader["PingMs"]),
            DnsSuccess = Convert.ToBoolean(reader["DnsSuccess"]),
            HttpSuccess = Convert.ToBoolean(reader["HttpSuccess"])
        };
    }

    public async Task<SpeedTestResult?> GetLatestSpeedTestAsync()
    {
        await using var connection = await _factory.OpenConnectionAsync();

        var command = connection.CreateCommand();

        command.CommandText =
        """
    SELECT *
    FROM SpeedTests
    ORDER BY Timestamp DESC
    LIMIT 1;
    """;

        await using var reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new SpeedTestResult
        {
            Timestamp = DateTimeOffset
                .FromUnixTimeSeconds(Convert.ToInt64(reader["Timestamp"]))
                .DateTime,
            PingMs = Convert.ToDouble(reader["PingMs"]),
            Jitter = Convert.ToDouble(reader["Jitter"]),
            DownloadMbps = Convert.ToDouble(reader["DownloadMbps"]),
            UploadMbps = Convert.ToDouble(reader["UploadMbps"]),
            PacketLoss = Convert.ToDouble(reader["PacketLoss"]),
            PublicIp = reader["PublicIp"]?.ToString(),
            Isp = reader["Isp"]?.ToString(),
            ServerName = reader["ServerName"]?.ToString(),
            ServerLocation = reader["ServerLocation"]?.ToString(),
            Success = true
        };
    }

    #endregion
}
