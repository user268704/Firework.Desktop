namespace Firework.Server.Abstraction;

public interface IServiceManagerService
{
    public void StartServer();
    public void StopServer();
    public bool IsRunning();
}