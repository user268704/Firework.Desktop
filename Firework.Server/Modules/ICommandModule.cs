namespace Firework.Server.Modules;

public interface ICommandModule
{
    string Name => GetType().Name;
}

