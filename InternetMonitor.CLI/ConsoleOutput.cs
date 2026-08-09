using InternetMonitor.Core.Models;

internal static class ConsoleOutput
{
    public static void PrintSpeedTest(SpeedTestResult result)
    {
        Console.WriteLine();
        Console.WriteLine("Speedtest");
        Console.WriteLine("---------");
        Console.WriteLine($"Download : {result.DownloadMbps:N2} Mbit/s");
        Console.WriteLine($"Upload   : {result.UploadMbps:N2} Mbit/s");
        Console.WriteLine($"Ping     : {result.PingMs:N1} ms");
        Console.WriteLine();
    }

    public static void PrintConnectivity(ConnectivityResult result)
    {
        Console.WriteLine();
        Console.WriteLine("Connectivity");
        Console.WriteLine("------------");
        Console.WriteLine($"Online  : {(result.IsOnline ? "Ja" : "Nein")}");
        Console.WriteLine($"Ping    : {(result.PingSuccess ? $"{result.PingMs:N0} ms" : "Fehler")}");
        Console.WriteLine($"DNS     : {(result.DnsSuccess ? "OK" : "Fehler")}");
        Console.WriteLine();
    }

    public static void PrintMessage(string message)
    {
        Console.WriteLine(message);
    }

    public static void PrintError(string message)
    {
        var oldColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Fehler: {message}");
        
        Console.ForegroundColor = oldColor;
    }

    public static void PrintSuccess(string message)
    {
        var oldColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        
        Console.ForegroundColor = oldColor;
    }

    public static void PrintWarning(string message)
    {
        var oldColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);

        Console.ForegroundColor = oldColor;
    }
}