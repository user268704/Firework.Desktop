using Firework.Abstraction.Connection;
using Firework.Models.Server;

namespace Firework.Core.Connection;

public class ConnectionManager : IConnectionManager
{
    private static ConnectionInfo _connectionInfo;

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
        return _connectionInfo;
    }

    public void SetConnectionInfo(ConnectionInfo connectionInfo)
    {
        _connectionInfo = connectionInfo;

        if (OnConnectionChanged != null)
        {
            OnConnectionChanged(this, connectionInfo);
        }
    }

    public void ChangeState(ConnectionState state)
    {
        _connectionInfo.State = state;
    }

    public event EventHandler<ConnectionInfo> OnConnectionChanged;
}