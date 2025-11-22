namespace Firework.Server.Dto.Commands;

public sealed class CommandDescriptorDto
{
    public string Module { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ParametersType { get; init; } = "None";
}

