using Firework.Server.Dto.Commands;
using Firework.Server.Models.Clients;
using FluentResults;

namespace Firework.Server.Abstraction;

public interface ICommandDispatcher
{
    Task<Result<CommandExecutionResultDto>> ExecuteAsync(RpcCommandDto command, RegisteredClient client, CancellationToken cancellationToken = default);
}

