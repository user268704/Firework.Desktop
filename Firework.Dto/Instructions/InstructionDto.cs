namespace Firework.Dto.Instructions;

public class InstructionDto
{
    public string Service { get; set; }
    public string Action { get; set; }
    public List<ActionParameterDto> Parameters { get; set; }
}