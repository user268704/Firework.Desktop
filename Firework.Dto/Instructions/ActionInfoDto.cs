namespace Firework.Dto.Instructions;

public class ActionInfoDto
{
    public string Name { get; set; }
    public List<ActionParameterDto> Parameters { get; set; }
    public string Description { get; set; }
}