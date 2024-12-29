namespace Firework.Models.Server;

public enum ConnectionState
{
    Connected,
    Reconnecting,
    Disconnected,
    NotConnected,
}