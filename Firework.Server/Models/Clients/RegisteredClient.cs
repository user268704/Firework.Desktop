namespace Firework.Server.Models.Clients;

public sealed class RegisteredClient
{
    public Guid DeviceId { get; init; }

    public string DeviceName { get; init; } = string.Empty;
    
    public string Ip { get; set; } = string.Empty;
    
    public string Token { get; init; } = string.Empty;
    
    public DateTime RegisteredAtUtc { get; init; }
    
    public DateTime LastSeenUtc { get; set; }
}

