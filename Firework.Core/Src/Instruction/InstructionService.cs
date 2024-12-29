using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.Services;
using Firework.Core.Exceptions;
using Firework.Core.Services;
using Firework.Dto.Dto;
using Firework.Dto.Instructions;

namespace Firework.Core.Instruction;

public class InstructionService : IInstructionService
{
    private readonly IServiceManager _serviceManager;
    private readonly Dictionary<string, ServiceInfo> _servicesInfos;

    private const string FORBIDDEN_CHARS_REGEX = "^[^:,]*$";

    public InstructionService(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
        
        _servicesInfos = _serviceManager.GetAllServices();
    }
    
    
    public InstructionInfo CreateInstruction(string instruction)
    {
        try
        {
            var instructionDto = ParseInstruction(instruction);

            var instructionResult = new InstructionInfo
            {
                ServiceName = instructionDto.Service,
                Parameters = GetParameters(instruction),
                ActionInfo = GetActionInfo(instructionDto.Service, instructionDto.Action)
            };

            return instructionResult;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ParseInstructionException("Не удалось спарсить инструкцию");
        }
        catch (ParseInstructionException ex)
        {
            throw new ParseInstructionException(ex.Message);
        }
    }

    public InstructionInfo CreateInstruction(string service, string action)
    {
        try
        {
            return new InstructionInfo
            {
                ServiceName = service,
                ActionInfo = GetActionInfo(service, action),
                Parameters = new List<ActionParameterInfo>()
            };
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ParseInstructionException("Не удалось спарсить инструкцию");
        }
        catch (ParseInstructionException ex)
        {
            throw new ParseInstructionException(ex.Message);
        }
    }

    public InstructionInfo CreateInstruction(string service, string action, List<ActionParameterInfo> parameters)
    {
        try
        {
            parameters ??= new List<ActionParameterInfo>();

            return new InstructionInfo
            {
                ServiceName = service,
                ActionInfo = GetActionInfo(service, action),
                Parameters = parameters
            };
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ParseInstructionException("Не удалось спарсить инструкцию");
        }
        catch (ParseInstructionException ex)
        {
            throw new ParseInstructionException(ex.Message);
        }
    }

    private ActionInfo GetActionInfo(string serviceName, string actionName)
    {
        if (_servicesInfos.TryGetValue(serviceName, out var serviceInfo))
        {
            var action = serviceInfo.ActionInfo.FirstOrDefault(x => x.Name == actionName);

            if (action == null)
                throw new ParseInstructionException("Такого действия не существует");

            return action;
        }

        throw new ParseInstructionException("Такого сервиса не существует");
    }

    private InstructionDto ParseInstruction(string instruction)
    {
        if (!IsValid(instruction))
            throw new ParseInstructionException("Инструкция не прошла валидацию");
        
        var result = new InstructionDto
        {
            Service = GetServiceName(instruction),
            Action = GetActionName(instruction).ToLower(),
            Parameters = GetParameters(instruction)
        };

        return result;
    }

    private string GetActionName(string instruction)
    {
        var action = instruction.Split('>')[1];
        
        return action.Contains('(') || action.Contains(')')
            ? action.Substring(0, action.IndexOf('('))
            : action;
    }
    
    private string GetServiceName(string instruction) => instruction.Split('>')[0];

    private bool IsValid(string instruction)
    {

        List<string> regexList = new()
        {
            // Один или несколько параметров но без именования
            // task>run(cmd.exe, vivaldi.exe)
            // task>run(cmd.exe)
            // mouse>click(left, 1)
            @"^\w+>\w+\(\w+(\.\w+)?(, \w+(\.\w+)?)*\)$",
            
            // Без параметров и без скобок
            // task>run
            @"^\w+>\w+$",
            
            // Без параметров но со скобками
            // task>run()
            @"^\w+>\w+\(\)$",
            
            // Именованные параметры
            // task>run(path: cmd.exe, arguments: 1)
            @"^\w+>\w+\(\w+: \w+(\.\w+)?, \w+: \w+(\.\w+)?\)$",
            
            // Именованные параметры с неограниченным количеством параметров
            // task>run(path: cmd.exe, arguments: 1, test: 1)
            @"^\w+>\w+\((\w+: \w+(\.\w+)?|\w+: \d+)(, \w+: \w+(\.\w+)?|\w+: \d+)*\)$"
        };
        
        List<bool> validationResults = new();
        
        foreach (string regex in regexList) 
            validationResults.Add(Regex.IsMatch(instruction, regex));
        
        return validationResults.Any(x =>  x);
    }

    private List<ActionParameterInfo> GetParameters(string instruction)
    {
        if (!instruction.Contains('(') && !instruction.Contains(')'))
            return new();
        
        List<ActionParameterInfo> result = new();
        instruction = instruction.Remove(0, instruction.IndexOf('(') + 1);
        instruction = instruction[..instruction.LastIndexOf(')')];

        var parameters = instruction.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var parameter in parameters)
        {
            var item = parameter.Split(':');

            if (item.Length <= 1)
                throw new ParseInstructionException("Не указано имя параметра");

            result.Add(new ActionParameterInfo(item.First().Trim(), item.Last().Trim()));
        }

        return result;
    }
    
    public string ToString(InstructionInfo instruction)
    {
        var resultInstructionString = new StringBuilder();

        resultInstructionString.Append($"{instruction.ServiceName}>");
        resultInstructionString.Append($"{instruction.ActionInfo.Name}");

        resultInstructionString.Append("(");
        if (instruction.Parameters.Count > 0)
            resultInstructionString.Append(string.Join(", ", instruction.Parameters.Select(x =>
                string.IsNullOrEmpty(x.Name)
                    ? $"{x.Value}"
                    : $"{x.Name}: {x.Value}")));


        resultInstructionString.Append(")");

        return resultInstructionString.ToString();
    }
}