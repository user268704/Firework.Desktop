namespace Firework.Server.Dto.Register;

public sealed class RegisterResponseDto
{
    public string Token { get; init; } = string.Empty;
    public Guid DeviceId { get; init; }
    public string DeviceName { get; init; } = string.Empty;
    public string NextAccessCode { get; init; } = string.Empty;
}

