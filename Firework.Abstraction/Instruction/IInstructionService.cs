using Firework.Models.Instructions;

namespace Firework.Abstraction.Instruction;

public interface IInstructionService
{
    public InstructionInfo CreateInstruction(string instruction);   
    public InstructionInfo CreateInstruction(string service, string action);   
    public InstructionInfo CreateInstruction(string service, string action, List<ActionParameterInfo> parameters);   
    public string ToString(InstructionInfo instruction);
}