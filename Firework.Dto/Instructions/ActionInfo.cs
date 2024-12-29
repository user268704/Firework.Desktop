using System.Reflection;
using System.Text.Json.Serialization;

namespace Firework.Dto.Instructions;

public class ActionInfo
{
    /*
    
    mouse>click:(left:2)
    
    here ->
    mouse == Name
    left:2 == Parameters
    click == Method
    
    Name - имя метода который будет обрабатывать запрос
    Parameters - имена параметров метода
    Method - объект для запуска метода
    */
    
    public string Name { get; set; }
    public List<ActionParameterInfo> Parameters { get; set; }
    
    [JsonIgnore]
    public MethodInfo Method { get; set; }
}