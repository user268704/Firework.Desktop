using System.Text.Json.Serialization;
using Firework.Models.Instructions;

namespace Firework.Models.Services;

public class ServiceInfo
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public List<ActionInfo> ActionInfo { get; set; }
    
    [JsonIgnore]
    public Type Type { get; set; }
}