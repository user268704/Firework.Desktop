using Firework.Dto.Instructions;
using Firework.Models.Instructions;
using FluentResults;

namespace Firework.Abstraction.Instruction;

public interface IInstructionService
{
    public InstructionInfo CreateInstruction(string instruction);   
    public IResult<InstructionInfo> CreateInstruction(string service, string action);   
    public IResult<InstructionInfo> CreateInstruction(string service, string action, IEnumerable<ActionParameterInfo> parameters);   
    public string ToString(InstructionInfo instruction);
}