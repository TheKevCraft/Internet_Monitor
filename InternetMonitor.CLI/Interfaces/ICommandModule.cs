using System.CommandLine;

public interface ICommandModule
{
    Command Create();
}