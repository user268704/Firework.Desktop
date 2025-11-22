using Firework.Server.Abstraction;
using Firework.Server.Dto.Commands;
using Firework.Server.Dto.Register;
using Firework.Server.Filters;
using FluentResults;

namespace Firework.Server.Endpoints;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("register", RegisterAsync)
            .WithName("Register");

        app.MapPost("command", ExecuteCommandAsync)
            .AddEndpointFilter<ClientAuthorizationEndpointFilter>()
            .WithName("ExecuteCommand");

        app.MapGet("updates", GetUpdatesAsync)
            .AddEndpointFilter<ClientAuthorizationEndpointFilter>()
            .WithName("GetUpdates");

        app.MapGet("commands", GetCommandsAsync)
            .AddEndpointFilter<ClientAuthorizationEndpointFilter>()
            .WithName("GetCommands");

        return app;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequestDto request,
        IRegistrationService registrationService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await registrationService.RegisterAsync(request, httpContext, cancellationToken);
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.BadRequest(result.Errors.First().Message);
    }

    private static async Task<IResult> ExecuteCommandAsync(
        HttpContext httpContext,
        ICommandDispatcher commandDispatcher,
        IMessagePackService messagePackService,
        CancellationToken cancellationToken)
    {
        if (!ClientAuthorizationFilter.TryGetClient(httpContext, out var client))
        {
            return Results.Unauthorized();
        }

        await using var memory = new MemoryStream();
        await httpContext.Request.Body.CopyToAsync(memory, cancellationToken);

        var commandResult = messagePackService.Deserialize<RpcCommandDto>(memory.ToArray());

        if (commandResult.IsFailed)
        {
            return Results.BadRequest(commandResult.Errors.First().Message);
        }

        var executionResult = await commandDispatcher.ExecuteAsync(
            commandResult.Value, 
            client, 
            cancellationToken);

        var payload = executionResult.IsSuccess
            ? executionResult.Value
            : new CommandExecutionResultDto
            {
                Module = commandResult.Value.ModuleName,
                Action = commandResult.Value.ActionName,
                IsSuccess = false,
                Message = executionResult.Errors.First().Message
            };

        var serialized = messagePackService.Serialize(payload);

        if (serialized.IsFailed)
        {
            return Results.Problem("Failed to serialize command result.", statusCode: 500);
        }

        return Results.File(serialized.Value, "application/x-msgpack");
    }

    private static async Task<IResult> GetUpdatesAsync(
        ISystemStateService systemStateService,
        CancellationToken cancellationToken)
    {
        var result = await systemStateService.GetCurrentStateAsync(cancellationToken);
        return MapResult(result);
    }

    private static async Task<IResult> GetCommandsAsync(
        int page,
        int pageSize,
        ISystemStateService systemStateService,
        CancellationToken cancellationToken)
    {
        var result = await systemStateService.GetCommandsAsync(page, pageSize, cancellationToken);
        return MapResult(result);
    }

    private static IResult MapResult<T>(Result<T> result)
    {
        return result.IsSuccess 
            ? Results.Ok(result.Value) 
            : Results.BadRequest(result.Errors.First().Message);
    }
}