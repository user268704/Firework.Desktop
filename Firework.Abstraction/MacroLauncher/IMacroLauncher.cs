using Firework.Dto.Instructions;
using Firework.Dto.Results;
using Firework.Models.Instructions;
using FluentResults;

namespace Firework.Abstraction.MacroLauncher;

public interface IMacroLauncher
{
    IResult<List<InstructionResult>> StartRange(List<InstructionInfo> macro);
    IResult<InstructionResult> Start(InstructionInfo macro);
}
