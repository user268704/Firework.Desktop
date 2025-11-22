using System.Linq;
using System.Reflection;
using Firework.Server.Abstraction;
using Firework.Server.Dto.Commands;
using Firework.Server.Models.Commands;
using Firework.Server.Modules;
using Firework.Server.Modules.Attributes;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Firework.Server.Services;

public sealed class CommandModuleRegistry : ICommandModuleRegistry
{
    private readonly Dictionary<string, CommandModuleDescriptor> _modules = new(StringComparer.OrdinalIgnoreCase);

    public CommandModuleRegistry(IEnumerable<Type> moduleTypes, ILogger<CommandModuleRegistry> logger)
    {
        foreach (var type in moduleTypes)
        {
            if (!typeof(ICommandModule).IsAssignableFrom(type))
            {
                logger.LogWarning("Type {Type} does not implement ICommandModule and will be ignored.", type.Name);
                continue;
            }

            var moduleName = type.Name;
            var actions = DiscoverActions(type, logger);

            var descriptor = new CommandModuleDescriptor
            {
                Name = moduleName,
                ModuleType = type,
                Actions = actions
            };

            _modules[moduleName] = descriptor;
        }

        logger.LogInformation("Registered {Count} command modules.", _modules.Count);
    }

    public Result<CommandModuleDescriptor> GetModule(string moduleName)
    {
        if (string.IsNullOrWhiteSpace(moduleName))
        {
            return Result.Fail("Module name is required.");
        }

        return _modules.TryGetValue(moduleName, out var descriptor)
            ? Result.Ok(descriptor)
            : Result.Fail("Module not found.");
    }

    public IEnumerable<CommandDescriptorDto> GetCommandDescriptors()
    {
        foreach (var module in _modules.Values)
        {
            foreach (var action in module.Actions.Values)
            {
                yield return new CommandDescriptorDto
                {
                    Module = module.Name,
                    Action = action.Name,
                    Description = action.Description,
                    ParametersType = action.ParameterType?.Name ?? "None"
                };
            }
        }
    }

    private static IReadOnlyDictionary<string, CommandActionDescriptor> DiscoverActions(Type moduleType, ILogger logger)
    {
        var actions = new Dictionary<string, CommandActionDescriptor>(StringComparer.OrdinalIgnoreCase);

        foreach (var method in moduleType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var attribute = method.GetCustomAttribute<CommandActionAttribute>();

            if (attribute == null)
            {
                continue;
            }

            ValidateMethodSignature(method);

            var parameterType = ExtractParameterType(method);

            if (attribute.ParametersType != null && attribute.ParametersType != parameterType)
            {
                throw new InvalidOperationException(
                    $"Method {method.Name} parameter type does not match attribute definition.");
            }

            var descriptor = new CommandActionDescriptor
            {
                Name = attribute.ActionName,
                Description = attribute.Description,
                MethodInfo = method,
                ParameterType = attribute.ParametersType ?? parameterType,
                AcceptsCancellationToken = MethodAcceptsCancellationToken(method)
            };

            actions[descriptor.Name] = descriptor;
        }

        logger.LogInformation("Module {Module} exposes {Count} actions.", moduleType.Name, actions.Count);
        return actions;
    }

    private static void ValidateMethodSignature(MethodInfo methodInfo)
    {
        if (methodInfo.ReturnType != typeof(Task<Result<object?>>))
        {
            throw new InvalidOperationException(
                $"Method {methodInfo.Name} must return Task<Result<object?>>.");
        }

        var parameters = methodInfo.GetParameters();

        if (parameters.Length == 0 || parameters[0].ParameterType != typeof(CommandContext))
        {
            throw new InvalidOperationException(
                $"Method {methodInfo.Name} must accept CommandContext as the first parameter.");
        }
    }

    private static bool MethodAcceptsCancellationToken(MethodInfo methodInfo)
    {
        return methodInfo.GetParameters().Any(parameter => parameter.ParameterType == typeof(CancellationToken));
    }

    private static Type? ExtractParameterType(MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters()[1..];
        Type? parameterType = null;

        foreach (var parameter in parameters)
        {
            if (parameter.ParameterType == typeof(CancellationToken))
            {
                continue;
            }

            if (parameterType != null)
            {
                throw new InvalidOperationException($"Method {methodInfo.Name} may only define a single parameter DTO.");
            }

            parameterType = parameter.ParameterType;
        }

        return parameterType;
    }
}

