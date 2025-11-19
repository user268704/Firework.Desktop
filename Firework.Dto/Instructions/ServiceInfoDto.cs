namespace Firework.Dto.Instructions;

public class ServiceInfoDto
{
    public string Name { get; set; }
    public List<ActionInfoDto> Actions { get; set; }
    public string Description { get; set; }
}