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

    public string Value { get; init; }
}