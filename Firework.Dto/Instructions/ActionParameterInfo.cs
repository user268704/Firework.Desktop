namespace Firework.Models.Instructions;

public class ActionParameterInfo
{
    
    public ActionParameterInfo(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public ActionParameterInfo()
    {
        
    }
    
    public string Name { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
}