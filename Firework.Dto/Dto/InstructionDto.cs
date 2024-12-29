using Firework.Dto.Instructions;

namespace Firework.Dto.Dto;

public class InstructionDto
{
    public string Service { get; set; }
    public string Action { get; set; }
    public List<ActionParameterInfo> Parameters { get; set; }
}