using InternetMonitor.Core.Config;
using InternetMonitor.Core.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace InternetMonitor.Core.Data;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly SqliteConnectionFactory _factory;

    public DatabaseInitializer(ILogger<DatabaseInitializer> logger, SqliteConnectionFactory factory)
    {
        _logger = logger;
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await using var connection = await _factory.OpenConnectionAsync();

        var command = connection.CreateCommand();

        command.CommandText = 
        """
        PRAGMA journal_mode=WAL;
        PRAGMA synchronous=NORMAL;

        CREATE TABLE IF NOT EXISTS ConnectivityResults
        (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Timestamp INTEGER NOT NULL,
            IsOnline INTEGER NOT NULL,
            PingSuccess INTEGER NOT NULL,
            PingMs INTEGER,
            DnsSuccess INTEGER NOT NULL,
            HttpSuccess INTEGER NOT NULL
        );

        CREATE TABLE IF NOT EXISTS SpeedTests
        (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Timestamp INTEGER NOT NULL,

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

        CREATE INDEX IF NOT EXISTS IX_ConnectivityResults_Timestamp
        ON ConnectivityResults(Timestamp DESC);

        CREATE INDEX IF NOT EXISTS IX_SpeedTests_Timestamp
        On SpeedTests(Timestamp DESC);
        """;

        try
        {
            await command.ExecuteNonQueryAsync();
        
            _logger.LogInformation(
                "Database initialized successfully");
        }
        catch(Exception ex)
        {
            _logger.LogError(
                ex,
                "Database initialization failed");
        
            throw;
        }
    }
}