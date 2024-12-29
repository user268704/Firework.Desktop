namespace Firework.Models.Server;

public enum ConnectionState
{
    Connected,
    Connecting,
    Reconnecting,
    Disconnected,
    NotConnected
}