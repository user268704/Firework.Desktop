namespace Firework.Server.Dto.Commands.Parameters;

public sealed class EchoCommandParams
{
    public string Message { get; init; } = string.Empty;
    public bool Uppercase { get; init; }
}

