using Firework.Models.Instructions;

namespace Firework.Abstraction.Connection;

public interface IConnectionService
{
    public void StartServer();
    public void SendInstruction(string endpoint, InstructionInfo instruction);

    public void SetFilter(string endpoint, IConnectionFilter filter);
    public void StopServer();
    public void RestartServer();
}