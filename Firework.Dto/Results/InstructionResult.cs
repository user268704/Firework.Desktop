using System.Net;

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

    public HttpStatusCode Status { get; set; }
    public string Value { get; init; }
}