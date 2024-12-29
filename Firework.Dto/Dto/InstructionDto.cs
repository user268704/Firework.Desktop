using System.Reflection;
using Firework.Models.Instructions;

namespace Firework.Models.Dto;

public class InstructionDto
{
    public string Service { get; set; }
    public string Action { get; set; }
    public List<ActionParameterInfo> Parameters { get; set; }
}