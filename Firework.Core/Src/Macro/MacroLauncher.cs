using Firework.Abstraction.MacroLauncher;
using Firework.Abstraction.Services;
using Firework.Dto.Results;
using Firework.Models.Instructions;
using FluentResults;

namespace Firework.Core.Macro;

public class MacroLauncher : IMacroLauncher
{
    private readonly IServiceManager _serviceManager;

    public MacroLauncher(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    
    public IResult<InstructionResult> Start(InstructionInfo macro)
    {
        if (macro == null)
            throw new NullReferenceException(nameof(macro));
        
        try
        {
            var service = _serviceManager.CreateService(macro.ServiceName);

            var resultString = service.Start(macro);

            var result = new InstructionResult(resultString.Value);

            return Result.Ok(result);
        }
        catch (NullReferenceException e)
        {
            return Result.Fail<InstructionResult>(e.Message);
        }
    }

    public IResult<List<InstructionResult>> StartRange(List<InstructionInfo> macro)
    {
        if (macro == null)
            throw new ArgumentOutOfRangeException(nameof(macro));
        
        List<InstructionResult> result = new();

        try
        {
            foreach (InstructionInfo instruction in macro)
            {
                var serviceResult = _serviceManager.CreateService(instruction.ServiceName).Start(instruction);

                result.Add(new InstructionResult(serviceResult.Value));
            }
        }
        catch (NullReferenceException e)
        {
            return Result.Fail<List<InstructionResult>>(e.Message);
        }

        return Result.Ok(result);
    }
    
    
}