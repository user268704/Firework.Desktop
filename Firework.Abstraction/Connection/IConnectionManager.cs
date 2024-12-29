using Firework.Models.Server;

namespace Firework.Abstraction.HttpServer;

public interface IConnectionManager
{
    ConnectionInfo Empty { get; }
    
    ConnectionInfo GetCurrentConnectionInfo();
    void SetConnectionInfo(ConnectionInfo connectionInfo);
    void ChangeState(ConnectionState state);
    event EventHandler<ConnectionInfo> OnConnectionChanged;
}
