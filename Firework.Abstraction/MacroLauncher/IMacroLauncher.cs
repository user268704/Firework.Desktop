using Firework.Dto.Results;
using Firework.Models.Instructions;

namespace Firework.Abstraction.MacroLauncher;

public interface IMacroLauncher
{
    List<InstructionResult> StartRange(List<InstructionInfo> macro);
    InstructionResult Start(InstructionInfo macro);
}
