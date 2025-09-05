namespace Firework.Dto.Results;

public class DeviceInfoHandshake
{
    public string DeviceName { get; set; } = string.Empty;
    public string Os { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string? ClientVersion { get; set; }
    public string? OperatingSystem { get; set; }
    public string? Architecture { get; set; }
}