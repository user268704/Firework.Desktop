using Firework.Dto.Instructions;

namespace Firework.Abstraction.Services;

public interface IServiceBase
{
    public string Start(InstructionInfo instruction);
    //protected string AutoStart(InstructionInfo instruction);
}