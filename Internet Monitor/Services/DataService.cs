using Internet_Monitor.Interfaces;
using Internet_Monitor.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

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

    public async Task SaveSpeedTestAsync(
    SpeedTestResult result)
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
}
