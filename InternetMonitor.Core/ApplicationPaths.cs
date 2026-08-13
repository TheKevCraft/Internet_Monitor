namespace InternetMonitor.Core;

public static class ApplicationPaths
{
    public static string AppDataDirectory
    {
        get
        {
            var basePath = OperatingSystem.IsWindows()
                ? Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData)
                : Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);

            return Path.Combine(basePath, "InternetMonitor");
        }
    }

    public static void Initialize()
    {
        Directory.CreateDirectory(AppDataDirectory);
    }

    public static string Resolve(string path)
    {
        if (Path.IsPathRooted(path))
            return path;

        return Path.GetFullPath(
            Path.Combine(AppDataDirectory, path));
    }
}
