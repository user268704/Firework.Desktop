namespace Firework.Dto.Instructions;

public class InstructionInfoDto
{
    public string ServiceName { get; set; }
    public string ActionName { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}