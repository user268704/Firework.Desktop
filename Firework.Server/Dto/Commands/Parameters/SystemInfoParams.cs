namespace Firework.Server.Dto.Commands.Parameters;

public sealed class SystemInfoParams
{
    public bool IncludeEnvironmentVariables { get; init; }
    public int EnvironmentVariableLimit { get; init; } = 5;
}

