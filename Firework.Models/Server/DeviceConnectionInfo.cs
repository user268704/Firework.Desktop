using MessagePack;

namespace Firework.Models.Server;

public class DeviceConnectionInfo : IEquatable<DeviceConnectionInfo>
{
    /// <summary>
    /// ID соединения SignalR
    /// </summary>
    public string ConnectionId { get; init; } = string.Empty;
    
    /// <summary>
    /// Имя устройства
    /// </summary>
    public string DeviceName { get; init; } = string.Empty;
    
    /// <summary>
    /// IP адрес устройства
    /// </summary>
    public string IpAddress { get; init; } = string.Empty;
    
    /// <summary>
    /// Состояние подключения
    /// </summary>
    public ConnectionState State { get; set; }
    
    /// <summary>
    /// Дата и время подключения
    /// </summary>
    public DateTime ConnectedAt { get; init; }
    
    /// <summary>
    /// Дата и время последней активности
    /// </summary>
    public DateTime LastActivity { get; set; }
    
    /// <summary>
    /// Дополнительные свойства устройства
    /// </summary>
    public Dictionary<string, object> Properties { get; set; } = new();
    
    /// <summary>
    /// Версия клиента
    /// </summary>
    public string? ClientVersion { get; set; }
    
    /// <summary>
    /// Операционная система устройства
    /// </summary>
    public string? OperatingSystem { get; set; }
    
    /// <summary>
    /// Архитектура процессора
    /// </summary>
    public string? Architecture { get; set; }

    public bool Equals(DeviceConnectionInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ConnectionId == other.ConnectionId && 
               DeviceName == other.DeviceName && 
               IpAddress == other.IpAddress;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((DeviceConnectionInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ConnectionId, DeviceName, IpAddress);
    }
    
    public override string ToString()
    {
        return $"Device: {DeviceName} ({IpAddress}) - {State}";
    }
}
