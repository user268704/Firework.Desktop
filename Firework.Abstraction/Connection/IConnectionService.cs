using Firework.Dto.Instructions;

namespace Firework.Abstraction.Connection;

public interface IConnectionService
{
    public void SendInstruction(string endpoint, InstructionInfo instruction); 
}