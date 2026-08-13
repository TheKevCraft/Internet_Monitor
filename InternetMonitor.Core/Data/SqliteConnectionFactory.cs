using InternetMonitor.Core.Configs.Options;
using Microsoft.Data.Sqlite;

namespace InternetMonitor.Core.Data;

public class SqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(MonitorOptions options)
    {
         var path = ApplicationPaths.Resolve(options.Database.Path);

        Directory.CreateDirectory(
            Path.GetDirectoryName(path)!);

        _connectionString =
            $"Data Source={path}";
    }

    public async Task<SqliteConnection> OpenConnectionAsync()
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}