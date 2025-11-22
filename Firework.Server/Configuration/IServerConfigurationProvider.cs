using FluentResults;

namespace Firework.Server.Configuration;

public interface IServerConfigurationProvider
{
    ServerConfiguration Current { get; }
    Task<Result<ServerConfiguration>> ReloadAsync(CancellationToken cancellationToken = default);
    Task<Result> PersistAsync(Action<ServerConfiguration> configure, CancellationToken cancellationToken = default);
}

