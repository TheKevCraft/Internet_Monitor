namespace Internet_Monitor.Services;

public static class Helper
{
    public static int GetLimit(string[] args, int index, int defaultLimit = 10)
    {
        return args.Length > index &&
            int.TryParse(args[index], out var parsed)
                ? parsed
                : defaultLimit;
    }
}
