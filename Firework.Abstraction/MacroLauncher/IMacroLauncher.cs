using Firework.Dto.Instructions;
using Firework.Dto.Results;

namespace Firework.Abstraction.MacroLauncher;

public interface IMacroLauncher
{
    List<InstructionResult> StartRange(List<InstructionInfo> macro);
    InstructionResult Start(InstructionInfo macro);
}
