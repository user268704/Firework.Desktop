namespace Firework.Server.Models.Commands;

public sealed class CommandModuleDescriptor
{
    public required string Name { get; init; }
    public required Type ModuleType { get; init; }
    public required IReadOnlyDictionary<string, CommandActionDescriptor> Actions { get; init; }
}

