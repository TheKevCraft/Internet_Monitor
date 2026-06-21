using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Microsoft.Data.Sqlite;

namespace Internet_Monitor.Services;

public class DataService : IDataService
{
    private readonly string _connectionString;

    public DataService()
    {
        var dataFolder = Path.Combine(
            AppContext.BaseDirectory,
            "Data");

        Directory.CreateDirectory(dataFolder);

        var dbPath = Path.Combine(
            dataFolder,
            "InternetMonitor.db");

        _connectionString = $"Data Source={dbPath}";
    }

    // Initialize
    public async Task InitializeAsync()
    {
        await using var connection = new SqliteConnection(_connectionString);

        await connection.OpenAsync();

        var command = connection.CreateCommand();

        command.CommandText =
        """
        CREATE TABLE IF NOT EXISTS ConnectivityResults
        (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Timestamp TEXT NOT NULL,
            IsOnline INTEGER NOT NULL,
            PingSuccess INTEGER NOT NULL,
            PingMs INTEGER,
            DnsSuccess INTEGER NOT NULL,
            HttpSuccess INTEGER NOT NULL
        );

        CREATE TABLE IF NOT EXISTS SpeedTests
        (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Timestamp TEXT NOT NULL,

            PingMs REAL NOT NULL,
            Jitter REAL NOT NULL,

            DownloadMbps REAL NOT NULL,
            UploadMbps REAL NOT NULL,

            PacketLoss REAL NOT NULL,

            PublicIp TEXT,

            Isp TEXT,
            ServerName TEXT,
            ServerLocation TEXT
        );
        """;

        await command.ExecuteNonQueryAsync();
    }

    // Saves
    public async Task SaveConnectivityAsync(ConnectivityResult result)
    {
        await using var connection =
        new SqliteConnection(_connectionString);

        await connection.OpenAsync();

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
            result.Timestamp);

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

        await command.ExecuteNonQueryAsync();
    }

    public async Task SaveSpeedTestAsync(SpeedTestResult result)
    {
        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync();

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

        command.Parameters.AddWithValue("$timestamp", result.Timestamp);
        command.Parameters.AddWithValue("$ping", result.PingMs);
        command.Parameters.AddWithValue("$jitter", result.Jitter);
        command.Parameters.AddWithValue("$download", result.DownloadMbps);
        command.Parameters.AddWithValue("$upload", result.UploadMbps);
        command.Parameters.AddWithValue("$packetLoss", result.PacketLoss);
        command.Parameters.AddWithValue("$publicIp", result.PublicIp ?? "");
        command.Parameters.AddWithValue("$isp", result.Isp ?? "");
        command.Parameters.AddWithValue("$serverName", result.ServerName ?? "");
        command.Parameters.AddWithValue("$serverLocation", result.ServerLocation ?? "");

        await command.ExecuteNonQueryAsync();
    }

    // Get`s
    public async Task<List<ConnectivityResult>> GetConnectivityHistoryAsync(int limit = 100)
    {
        var results = new List<ConnectivityResult>();

        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync();

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
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(new ConnectivityResult
            {
                Timestamp = DateTime.Parse(reader["Timestamp"].ToString()!),
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

    public async Task<List<SpeedTestResult>> GetSpeedTestHistoryAsync(int limit = 100)
    {
        var results = new List<SpeedTestResult>();

        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync();

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
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(new SpeedTestResult
            {
                Timestamp = DateTime.Parse(reader["Timestamp"].ToString()!),
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
        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync();

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
            Timestamp = DateTime.Parse(reader["Timestamp"].ToString()!),
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
        await using var connection =
            new SqliteConnection(_connectionString);

        await connection.OpenAsync();

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
            Timestamp = DateTime.Parse(reader["Timestamp"].ToString()!),
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
}
