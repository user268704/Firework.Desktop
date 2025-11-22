using Firework.Server.Abstraction;
using Firework.Server.Dto.Commands;
using Firework.Server.Dto.System;
using Firework.Server.Models.Clients;
using FluentResults;

namespace Firework.Server.Services;

public sealed class SystemStateService : ISystemStateService
{
    private readonly IClientRegistry _clientRegistry;
    private readonly ICommandModuleRegistry _commandModuleRegistry;
    private const int DefaultPageSize = 20;

    public SystemStateService(IClientRegistry clientRegistry, ICommandModuleRegistry commandModuleRegistry)
    {
        _clientRegistry = clientRegistry;
        _commandModuleRegistry = commandModuleRegistry;
    }

    public Task<Result<SystemStateDto>> GetCurrentStateAsync(CancellationToken cancellationToken = default)
    {
        var clientsResult = _clientRegistry.GetAll();

        if (clientsResult.IsFailed)
        {
            return Task.FromResult(clientsResult.ToResult<SystemStateDto>());
        }

        var state = new SystemStateDto
        {
            Clients = clientsResult.Value.Select(MapToClientDto).ToArray(),
            Commands = _commandModuleRegistry.GetCommandDescriptors().Take(DefaultPageSize).ToArray(),
            GeneratedAtUtc = DateTime.UtcNow,
            ServerVersion = typeof(SystemStateService).Assembly.GetName().Version?.ToString() ?? "unknown"
        };

        return Task.FromResult(Result.Ok(state));
    }

    public Task<Result<IReadOnlyList<CommandDescriptorDto>>> GetCommandsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page <= 0)
        {
            return Task.FromResult(Result.Fail<IReadOnlyList<CommandDescriptorDto>>("Page must be positive."));
        }

        if (pageSize <= 0)
        {
            pageSize = DefaultPageSize;
        }

        var skip = (page - 1) * pageSize;
        var commands = _commandModuleRegistry.GetCommandDescriptors().Skip(skip).Take(pageSize).ToArray();
        return Task.FromResult(Result.Ok<IReadOnlyList<CommandDescriptorDto>>(commands));
    }

    private static ClientDto MapToClientDto(RegisteredClient client)
    {
        return new ClientDto
        {
            DeviceId = client.DeviceId,
            DeviceName = client.DeviceName,
            Ip = client.Ip,
            RegisteredAt = client.RegisteredAtUtc,
            LastSeen = client.LastSeenUtc
        };
    }
}

