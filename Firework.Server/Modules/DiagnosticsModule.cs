using Firework.Server.Models.Commands;
using Firework.Server.Modules.Attributes;
using FluentResults;

namespace Firework.Server.Modules;

public sealed class DiagnosticsModule : ICommandModule
{
    [CommandAction("Ping", "Simple health-check command.")]
    public Task<Result<object?>> PingAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var payload = new
        {
            Message = "pong",
            ServerTimeUtc = DateTime.UtcNow
        };

        return Task.FromResult(Result.Ok<object?>(payload));
    }
}

