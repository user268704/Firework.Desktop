using MessagePack;

namespace Firework.Server.Dto.Commands;

[MessagePackObject]
public sealed class RpcCommandDto
{
    [Key(0)]
    public string ModuleName { get; init; } = string.Empty;

    [Key(1)]
    public string ActionName { get; init; } = string.Empty;

    [Key(2)]
    public byte[]? Params { get; init; }
}

