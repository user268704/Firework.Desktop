using Firework.Dto.Dto;

namespace Firework.Dto.Results;

public class InstructionResult
{

    public InstructionResult(string message)
    {
        Value = message;
    }

    public InstructionResult()
    {

    }

    public StatusCode Status { get; set; }
    public string Value { get; init; }
}