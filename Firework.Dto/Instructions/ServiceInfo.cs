using System.Text.Json.Serialization;

namespace Firework.Dto.Instructions;

public class ServiceInfo
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public List<ActionInfo> ActionInfo { get; set; }
    
    [JsonIgnore]
    public Type Type { get; set; }
}