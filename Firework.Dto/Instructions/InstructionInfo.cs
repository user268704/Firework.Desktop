namespace Firework.Dto.Instructions;

public class InstructionInfo
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ServiceName { get; set; }
    public ActionInfo ActionInfo { get; set; }
    public List<ActionParameterInfo> Parameters { get; set; }
}