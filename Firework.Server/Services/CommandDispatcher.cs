using System.Linq;
using Firework.Server.Abstraction;
using Firework.Server.Dto.Commands;
using Firework.Server.Models.Clients;
using Firework.Server.Models.Commands;
using Firework.Server.Modules;
using FluentResults;
using FluentValidation;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;

namespace Firework.Server.Services;

public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICommandModuleRegistry _moduleRegistry;
    private readonly IMessagePackService _messagePackService;
    private readonly IValidator<RpcCommandDto> _commandValidator;
    private readonly ILogger<CommandDispatcher> _logger;

    public CommandDispatcher(
        IServiceProvider serviceProvider,
        ICommandModuleRegistry moduleRegistry,
        IMessagePackService messagePackService,
        IValidator<RpcCommandDto> commandValidator,
        ILogger<CommandDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _moduleRegistry = moduleRegistry;
        _messagePackService = messagePackService;
        _commandValidator = commandValidator;
        _logger = logger;
    }

    public async Task<Result<CommandExecutionResultDto>> ExecuteAsync(RpcCommandDto command, RegisteredClient client, CancellationToken cancellationToken = default)
    {
        var validation = await _commandValidator.ValidateAsync(command, cancellationToken);

        if (!validation.IsValid)
        {
            return Result.Fail(validation.ToString());
        }

        var moduleResult = _moduleRegistry.GetModule(command.ModuleName);

        if (moduleResult.IsFailed)
        {
            return moduleResult.ToResult<CommandExecutionResultDto>();
        }

        var module = moduleResult.Value;

        if (!module.Actions.TryGetValue(command.ActionName, out var action))
        {
            return Result.Fail<CommandExecutionResultDto>($"Action {command.ActionName} not found in module {command.ModuleName}.");
        }

        var parametersResult = BuildParameters(action, command.Params);

        if (parametersResult.IsFailed)
        {
            return parametersResult.ToResult<CommandExecutionResultDto>();
        }

        await using var scope = _serviceProvider.CreateAsyncScope();
        var moduleInstance = (ICommandModule)scope.ServiceProvider.GetRequiredService(module.ModuleType);

        var invocationResult = await InvokeAsync(
            moduleInstance,
            action,
            new CommandContext(client),
            parametersResult.Value,
            cancellationToken);

        if (invocationResult.IsFailed)
        {
            _logger.LogWarning("Command {Module}.{Action} failed: {Error}", module.Name, action.Name,
                invocationResult.Errors.First().Message);
            
            return invocationResult.ToResult<CommandExecutionResultDto>();
        }

        return Result.Ok(new CommandExecutionResultDto
        {
            Module = module.Name,
            Action = action.Name,
            IsSuccess = true,
            Message = "Command executed successfully.",
            Payload = invocationResult.Value
        });
    }

    private Result<object?> BuildParameters(CommandActionDescriptor descriptor, byte[]? parametersBuffer)
    {
        if (descriptor.ParameterType == null)
        {
            return Result.Ok<object?>(null);
        }

        if (parametersBuffer is null || parametersBuffer.Length == 0)
        {
            return Result.Fail<object?>("Command parameters are required.");
        }

        try
        {
            return _messagePackService.Deserialize(parametersBuffer, descriptor.ParameterType);
        }
        catch (Exception exception)
        {
            return Result.Fail<object?>(
                new Error("Failed to deserialize command parameters.").CausedBy(exception));
        }
    }

    private async Task<Result<object?>> InvokeAsync(ICommandModule moduleInstance, CommandActionDescriptor descriptor, CommandContext context, object? parameters, CancellationToken cancellationToken)
    {
        try
        {
            var args = BuildArgumentArray(descriptor, context, parameters, cancellationToken);
            
            var task = (Task<Result<object?>>)descriptor.MethodInfo.Invoke(moduleInstance, args)!;
            
            return await task.ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Command invocation failed for {Module}.{Action}.", moduleInstance.GetType().Name, descriptor.Name);
            return Result.Fail<object?>(new Error("Command execution failed.").CausedBy(exception));
        }
    }

    private static object?[] BuildArgumentArray(CommandActionDescriptor descriptor, CommandContext context, object? parameters, CancellationToken cancellationToken)
    {
        var arguments = new List<object?> { context };

        if (descriptor.ParameterType != null)
        {
            arguments.Add(parameters);
        }

        if (descriptor.AcceptsCancellationToken)
        {
            arguments.Add(cancellationToken);
        }

        return arguments.ToArray();
    }
}

