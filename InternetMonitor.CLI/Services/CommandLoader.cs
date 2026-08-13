using System.CommandLine;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

public static class CommandLoader
{
    public static void RegisterCommands(RootCommand root, IServiceProvider services)
    {
        var modules = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                typeof(ICommandModule).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                !t.IsInterface);

        foreach (var type in modules)
        {
            var module = (ICommandModule)
                ActivatorUtilities.CreateInstance(services, type);

            root.Add(module.Create());
        }
    }
}