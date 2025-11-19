namespace Firework.Models.Devices;

public class Device
{
    public string Hash { get; set; }
    public string Name { get; set; }
    public DateTime LastUpdate { get; set; }
    public DateTime LastConnected { get; set; }
    public string OperatingSystem { get; set; }
    public string IP { get; set; }
    public bool IsMaster { get; set; }
}