namespace Firework.Server.Dto.System;

public sealed class ClientDto
{
    public Guid DeviceId { get; init; }
    public string DeviceName { get; init; } = string.Empty;
    public string Ip { get; init; } = string.Empty;
    public DateTime RegisteredAt { get; init; }
    public DateTime LastSeen { get; init; }
}

