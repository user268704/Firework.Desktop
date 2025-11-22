using System.Collections;
using Firework.Server.Dto.Commands.Parameters;
using Firework.Server.Models.Commands;
using Firework.Server.Modules.Attributes;
using FluentResults;

namespace Firework.Server.Modules;

/// <summary>
/// Example module demonstrating how to expose server-side capabilities to the RPC pipeline.
/// Register modules inside Program.cs through <c>services.AddScoped&lt;SystemModule&gt;();</c>
/// and add them to <c>AddCommandModules</c>.
/// </summary>
public sealed class SystemModule : ICommandModule
{
    private readonly ILogger<SystemModule> _logger;

    public SystemModule(ILogger<SystemModule> logger)
    {
        _logger = logger;
    }

    [CommandAction("GetServerInfo", "Returns basic system information.", typeof(SystemInfoParams))]
    public Task<Result<object?>> GetServerInfoAsync(CommandContext context, SystemInfoParams parameters, CancellationToken cancellationToken)
    {
        var info = new
        {
            Environment.MachineName,
            OSVersion = Environment.OSVersion.ToString(),
            ProcessId = Environment.ProcessId,
            UtcNow = DateTime.UtcNow,
            EnvironmentVariables = parameters.IncludeEnvironmentVariables
                ? Environment.GetEnvironmentVariables()
                               .Cast<DictionaryEntry>()
                               .Take(Math.Max(1, parameters.EnvironmentVariableLimit))
                               .ToDictionary(entry => entry.Key?.ToString() ?? string.Empty, entry => entry.Value?.ToString() ?? string.Empty)
                : null
        };

        _logger.LogInformation("Server info requested by {Device} ({Ip}).", context.Client.DeviceName, context.Client.Ip);
        return Task.FromResult(Result.Ok<object?>(info));
    }

    [CommandAction("Echo", "Returns message back to the caller.", typeof(EchoCommandParams))]
    public Task<Result<object?>> EchoAsync(CommandContext context, EchoCommandParams parameters, CancellationToken cancellationToken)
    {
        var message = parameters.Uppercase
            ? parameters.Message.ToUpperInvariant()
            : parameters.Message;

        var payload = new
        {
            parameters.Message,
            Echo = message,
            Device = context.Client.DeviceName,
            Timestamp = DateTime.UtcNow
        };

        return Task.FromResult(Result.Ok<object?>(payload));
    }
}

