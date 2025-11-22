namespace Firework.Server.Dto.Devices;

public sealed class DevicePayloadDto
{
    public Guid DeviceId { get; init; }
    public string DeviceName { get; init; } = string.Empty;
    public string Ip { get; init; } = string.Empty;
}

