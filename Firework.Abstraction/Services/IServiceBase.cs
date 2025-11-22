using Firework.Dto.Instructions;
using Firework.Models.Instructions;
using FluentResults;

namespace Firework.Abstraction.Services;

public interface IServiceBase
{
    public IResult<string> Start(InstructionInfo instruction);
    //protected string AutoStart(InstructionInfo instruction);
}