using Firework.Server.Abstraction;
using Firework.Server.Configuration;
using FluentResults;

namespace Firework.Server.Services;

public sealed class AccessCodeService : IAccessCodeService
{
    private readonly IServerConfigurationProvider _configurationProvider;
    private readonly ILogger<AccessCodeService> _logger;

    public AccessCodeService(IServerConfigurationProvider configurationProvider, ILogger<AccessCodeService> logger)
    {
        _configurationProvider = configurationProvider;
        _logger = logger;
    }

    public string CurrentCode => _configurationProvider.Current.Security.AccessCode ?? string.Empty;

    public Result EnsureValid(string submittedCode)
    {
        if (string.IsNullOrWhiteSpace(submittedCode))
        {
            return Result.Fail("Access code is required.");
        }

        if (string.Equals(CurrentCode, submittedCode, StringComparison.Ordinal))
        {
            return Result.Ok();
        }

        return Result.Fail("Access code is invalid.");
    }

    public async Task<Result<string>> RotateAsync(CancellationToken cancellationToken = default)
    {
        var newCode = GenerateCode();
        var persistResult = await _configurationProvider.PersistAsync(cfg => cfg.Security.AccessCode = newCode, cancellationToken);

        if (persistResult.IsFailed)
        {
            return persistResult.ToResult<string>();
        }

        _logger.LogInformation("Access code rotated.");
        return Result.Ok(newCode);
    }

    private static string GenerateCode()
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var buffer = new char[10];

        for (var index = 0; index < buffer.Length; index++)
        {
            buffer[index] = alphabet[Random.Shared.Next(alphabet.Length)];
        }

        return new string(buffer);
    }
}

