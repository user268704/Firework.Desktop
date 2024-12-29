namespace Firework.Dto.Instructions;

/*public class Instruction
{
    /*public List<ActionParameterInfo> Parameters { get; private init; }
    public ServiceInfo Service { get; private init; }
    public ActionInfo Action { get; private init; }#1#

    public Instruction(string service, string action, string parameters)
    {
        Service = service;
        Action = action;
        Parameters = GetParameters(parameters);
    }

    public Instruction(string service, string action)
    {
        Service = service;
        Action = action;
        Parameters = new List<ActionParameterInfo>();
    }

    public Instruction(string instruction)
    {
        var createdInstruction = CreateInstruction(instruction);
        
        Service = createdInstruction.Service;
        Action = createdInstruction.Action;
        Parameters = createdInstruction.Parameters;
    }

    public Instruction()
    { }

    public Instruction CreateInstruction(string instruction)
    {
        try
        {
            var serviceName = instruction[..instruction.IndexOf('>')];
            instruction = instruction.Remove(0, instruction.IndexOf('>') + 1);

            var instructionResult = new Instruction
            {
                Service = serviceName,
                Parameters = GetParameters(instruction),
                Action = instruction[..instruction.IndexOf('(')]
            };

            return instructionResult;
        }
        catch (ArgumentOutOfRangeException)
        {
            Debug.Print($"Ошибка парсинга инструкции: {instruction}");
            throw new ArgumentException("Instruction not valid", nameof(instruction));
        }
    }

    private List<ActionParameterInfo> GetParameters(string instruction)
    {
        List<ActionParameterInfo> result = new();
        instruction = instruction.Remove(0, instruction.IndexOf('(') + 1);
        instruction = instruction[..instruction.LastIndexOf(')')];

        if (string.IsNullOrEmpty(instruction))
            return new List<ActionParameterInfo>(0);

        var parameters = instruction.Split(',').ToList();
        foreach (var parameter in parameters)
        {
            var item = parameter.Split(':');
            result.Add(new ActionParameterInfo(item.Last() == item.First() ? null : item.First().Trim(), item.Last().Trim()));
        }

        return result;
    }


    /// <summary>   
    ///     Конвертирует инструкцию в строку
    /// </summary>
    /// <exception cref="Exception">Если главный модуль или действие не установлены</exception>
    /*
    public override string ToString()
    {
        if (string.IsNullOrEmpty(Action) || string.IsNullOrEmpty(Service))
            throw new Exception();

        var resultInstructionString = new StringBuilder();

        resultInstructionString.Append($"{Service}>");
        resultInstructionString.Append($"{Action}");

        resultInstructionString.Append("(");
        if (Parameters.Count > 0)
            for (var index = 0; index < Parameters.Count; index++)
            {
                var parameter = Parameters[index];
                if (Parameters.Count > index)
                    resultInstructionString.Append($"{parameter.Name}:{parameter.Value},");

                if (Parameters.Count == index)
                    resultInstructionString.Append($"{parameter.Name}:{parameter.Value}");
            }

        resultInstructionString.Append(")");

        return resultInstructionString.ToString();
    }
#1#
}*/