using System.Reflection;

namespace Firework.Server.Models.Commands;

public sealed class CommandActionDescriptor
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required MethodInfo MethodInfo { get; init; }
    public Type? ParameterType { get; init; }
    public bool AcceptsCancellationToken { get; init; }
}

