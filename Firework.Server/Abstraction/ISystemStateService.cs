using Firework.Server.Dto.Commands;
using Firework.Server.Dto.System;
using FluentResults;

namespace Firework.Server.Abstraction;

public interface ISystemStateService
{
    Task<Result<SystemStateDto>> GetCurrentStateAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<CommandDescriptorDto>>> GetCommandsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}

