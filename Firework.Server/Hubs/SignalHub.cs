using Firework.Server.Abstraction;
using Firework.Server.Dto.Commands;
using Firework.Server.Dto.System;
using Firework.Server.Models.Clients;
using Microsoft.AspNetCore.SignalR;

namespace Firework.Server.Hubs;

public sealed class SignalHub : Hub
{
    private readonly IClientRegistry _clientRegistry;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IMessagePackService _messagePackService;
    private readonly ISystemStateService _systemStateService;

    private const string ClientKey = "hub-client";

    public SignalHub(
        IClientRegistry clientRegistry,
        ICommandDispatcher commandDispatcher,
        IMessagePackService messagePackService,
        ISystemStateService systemStateService)
    {
        _clientRegistry = clientRegistry;
        _commandDispatcher = commandDispatcher;
        _messagePackService = messagePackService;
        _systemStateService = systemStateService;
    }

    public override Task OnConnectedAsync()
    {
        var token = Context.GetHttpContext()?.Request.Query["token"].ToString();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new HubException("Authentication token is required.");
        }

        var clientResult = _clientRegistry.GetByToken(token);

        if (clientResult.IsFailed)
        {
            throw new HubException("Invalid authentication token.");
        }

        Context.Items[ClientKey] = clientResult.Value;
        return base.OnConnectedAsync();
    }

    public async Task<byte[]> Command(byte[] input, CancellationToken cancellationToken)
    {
        var client = GetClient();
        var commandResult = _messagePackService.Deserialize<RpcCommandDto>(input);

        if (commandResult.IsFailed)
        {
            throw new HubException(commandResult.Errors.First().Message);
        }

        var executionResult = await _commandDispatcher.ExecuteAsync(commandResult.Value, client, cancellationToken);

        var payload = executionResult.IsSuccess
            ? executionResult.Value
            : new CommandExecutionResultDto
            {
                Module = commandResult.Value.ModuleName,
                Action = commandResult.Value.ActionName,
                IsSuccess = false,
                Message = executionResult.Errors.First().Message
            };

        var serialized = _messagePackService.Serialize(payload);

        if (serialized.IsFailed)
        {
            throw new HubException("Failed to serialize command response.");
        }

        return serialized.Value;
    }

    public async Task<SystemStateDto> Updates(CancellationToken cancellationToken)
    {
        var result = await _systemStateService.GetCurrentStateAsync(cancellationToken);

        if (result.IsFailed)
        {
            throw new HubException(result.Errors.First().Message);
        }

        return result.Value;
    }

    private RegisteredClient GetClient()
    {
        if (Context.Items.TryGetValue(ClientKey, out var client) && client is RegisteredClient registeredClient)
        {
            return registeredClient;
        }

        throw new HubException("Client is not authenticated.");
    }
}