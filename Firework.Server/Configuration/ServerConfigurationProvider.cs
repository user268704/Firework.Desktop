using System.Text.Json;
using FluentResults;
using Microsoft.Extensions.Hosting;

namespace Firework.Server.Configuration;

public sealed class ServerConfigurationProvider : IServerConfigurationProvider
{
    private readonly ILogger<ServerConfigurationProvider> _logger;
    private readonly string _configPath;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };
    private readonly SemaphoreSlim _syncRoot = new(1, 1);

    public ServerConfigurationProvider(IHostEnvironment environment, ILogger<ServerConfigurationProvider> logger)
    {
        _logger = logger;
        _configPath = Path.Combine(environment.ContentRootPath, "serverconfig.json");
        Current = LoadOrCreate();
    }

    public ServerConfiguration Current { get; private set; }

    public async Task<Result<ServerConfiguration>> ReloadAsync(CancellationToken cancellationToken = default)
    {
        await _syncRoot.WaitAsync(cancellationToken);

        try
        {
            Current = LoadOrCreate();
            return Result.Ok(Current);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to reload server configuration.");
            return Result.Fail(new Error("Unable to reload configuration.").CausedBy(exception));
        }
        finally
        {
            _syncRoot.Release();
        }
    }

    public async Task<Result> PersistAsync(Action<ServerConfiguration> configure, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configure);

        await _syncRoot.WaitAsync(cancellationToken);

        try
        {
            var clone = Current.Clone();
            configure(clone);
            clone.Security.EnsureAccessCode();

            var directory = Path.GetDirectoryName(_configPath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var stream = File.Open(_configPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            await JsonSerializer.SerializeAsync(stream, clone, _serializerOptions, cancellationToken);
            await stream.FlushAsync(cancellationToken);

            Current = clone;
            _logger.LogInformation("Server configuration persisted to {Path}.", _configPath);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to persist configuration.");
            return Result.Fail(new Error("Unable to persist configuration.").CausedBy(exception));
        }
        finally
        {
            _syncRoot.Release();
        }
    }

    private ServerConfiguration LoadOrCreate()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                var config = ServerConfiguration.CreateDefault();
                
                using var stream = File.Open(_configPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                JsonSerializer.Serialize(stream, config, _serializerOptions);
                
                _logger.LogInformation("Created default server configuration at {Path}.", _configPath);
            
                return config;
            }

            using var fileStream = File.OpenRead(_configPath);
            
            var existing = JsonSerializer.Deserialize<ServerConfiguration>(fileStream, _serializerOptions)
                            ?? ServerConfiguration.CreateDefault();
            existing.Security.EnsureAccessCode();
          
            return existing;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to load configuration, falling back to defaults.");
            
            var fallback = ServerConfiguration.CreateDefault();
            fallback.Security.EnsureAccessCode();
            
            return fallback;
        }
    }
}

