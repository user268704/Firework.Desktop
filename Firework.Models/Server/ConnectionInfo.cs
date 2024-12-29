using MessagePack;

namespace Firework.Models.Server;

public class ConnectionInfo : IEquatable<ConnectionInfo>
{
    public string ClientName { get; init; }

    public string ClientIp { get; init; }

    public bool IsConnected { get; set; }
    public ConnectionState State { get; set; }

    public DateTime DateConnected { get; init; }

    public bool Equals(ConnectionInfo? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ClientName == other.ClientName && ClientIp == other.ClientIp && IsConnected == other.IsConnected;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ConnectionInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ClientName, ClientIp, IsConnected);
    }
}