namespace Firework.Server.Modules.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class CommandActionAttribute : Attribute
{
    public CommandActionAttribute(string actionName, string description, Type? parametersType = null)
    {
        ActionName = actionName;
        Description = description;
        ParametersType = parametersType;
    }

    public string ActionName { get; }
    public string Description { get; }
    public Type? ParametersType { get; }
}

