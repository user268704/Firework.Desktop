using Firework.Abstraction.MacroLauncher;
using Firework.Abstraction.Services;
using Firework.Core.Services;
using Firework.Dto.Results;
using Firework.Models;
using Firework.Models.Instructions;

namespace Firework.Core.Macro;

public class MacroLauncher : IMacroLauncher
{
    private readonly IServiceManager _serviceManager;

    public MacroLauncher(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    
    public InstructionResult Start(InstructionInfo macro)
    {
        if (macro == null)
            throw new NullReferenceException(nameof(macro));
        
        try
        {
            var service = _serviceManager.CreateService(macro.ServiceName);

            var resultString = service.Start(macro);

            var result = new InstructionResult(resultString);

            return result;
        }
        catch (NullReferenceException e)
        {
            return new(e.Message);
        }
    }

    public List<InstructionResult> StartRange(List<InstructionInfo> macro)
    {
        if (macro == null)
            throw new ArgumentOutOfRangeException(nameof(macro));
        
        List<InstructionResult> result = new();

        try
        {
            foreach (InstructionInfo instruction in macro)
            {
                var serviceResult = _serviceManager.CreateService(instruction.ServiceName).Start(instruction);

                result.Add(new InstructionResult(serviceResult));
            }
        }
        catch (NullReferenceException e)
        {
            throw e;
        }

        return result;
    }
    
    
}