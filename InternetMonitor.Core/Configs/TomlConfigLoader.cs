using InternetMonitor.Core.Configs.Options;
using Tomlyn;

namespace InternetMonitor.Core.Configs;

internal static class TomlConfigLoader
{
    public static MonitorOptions Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Configuration file was not found: {path}",
                path);
        }

        var toml = File.ReadAllText(path);

        try
        {
            return TomlSerializer.Deserialize<MonitorOptions>(toml);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load configuration from '{path}'.",
                ex);
        }
    }
}
