namespace InternetMonitor.Core.Config;

public class DatabaseOptions
{
    public string Provider { get; set; } = "SQLite";

    public string Path { get; set; } = "./data/InternetMonitor.db";
}