using Firework.Abstraction.Connection;
using Firework.Models.Server;
using ConnectionInfo = Firework.Models.Server.ConnectionInfo;

namespace Firework.Server.Hubs;

public class ConnectionManager : IConnectionManager
{
    private ConnectionInfo _connectionInfo;
    private readonly object _lock = new object();

    public event EventHandler<ConnectionInfo> OnConnectionChanged;

    public ConnectionInfo Empty =>
        new()
        {
            DateConnected = DateTime.MinValue,
            ClientIp = "0.0.0.0",
            ClientName = "Нет подключения",
            IsConnected = false,
            State = ConnectionState.NotConnected
        };

    public ConnectionManager()
    {
        _connectionInfo = Empty;
    }

    public ConnectionInfo GetCurrentConnectionInfo()
    {
        lock (_lock)
        {
            return _connectionInfo;
        }
    }

    public void SetConnectionInfo(ConnectionInfo connectionInfo)
    {
        lock (_lock)
        {
            _connectionInfo = connectionInfo;
        }

        OnConnectionChanged?.Invoke(this, connectionInfo);
    }

    public void ChangeState(ConnectionState state)
    {
        lock (_lock)
        {
            _connectionInfo.State = state;
        }
    }
}