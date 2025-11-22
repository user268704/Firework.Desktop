using Firework.Server.Dto.Devices;

namespace Firework.Server.Dto.Register;

public sealed class RegisterRequestDto
{
    public string AccessCode { get; init; } = string.Empty;
    public DevicePayloadDto Payload { get; init; } = new();
}

