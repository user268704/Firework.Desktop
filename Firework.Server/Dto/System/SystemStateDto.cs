using Firework.Server.Dto.Commands;

namespace Firework.Server.Dto.System;

public sealed class SystemStateDto
{
    public IReadOnlyCollection<ClientDto> Clients { get; init; } = Array.Empty<ClientDto>();
    public IReadOnlyCollection<CommandDescriptorDto> Commands { get; init; } = Array.Empty<CommandDescriptorDto>();
    public DateTime GeneratedAtUtc { get; init; } = DateTime.UtcNow;
    public string ServerVersion { get; init; } = string.Empty;
}

