using InternetMonitor.Core.Config;
using Microsoft.Data.Sqlite;

namespace InternetMonitor.Core.Data;

public class SqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(DatabaseOptions options)
    {
         var path = Path.GetFullPath(options.Path);

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