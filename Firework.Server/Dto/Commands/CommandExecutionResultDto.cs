namespace Firework.Server.Dto.Commands;

public sealed class CommandExecutionResultDto
{
    public string Module { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public object? Payload { get; init; }
}

