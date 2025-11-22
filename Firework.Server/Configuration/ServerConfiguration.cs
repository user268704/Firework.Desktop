using System.Text.Json.Serialization;

namespace Firework.Server.Configuration;

public sealed class ServerConfiguration
{
    private const string DefaultLogPath = "logs/server.log";

    [JsonPropertyName("logging")]
    public LoggingOptions Logging { get; init; } = new()
    {
        Enabled = true,
        LogToConsole = true,
        LogToFile = true,
        FilePath = DefaultLogPath
    };

    [JsonPropertyName("security")]
    public SecurityOptions Security { get; init; } = new();

    public static ServerConfiguration CreateDefault()
    {
        var config = new ServerConfiguration();
        config.Security.EnsureAccessCode();
        config.Logging.FilePath ??= DefaultLogPath;
        return config;
    }

    public ServerConfiguration Clone()
    {
        return new ServerConfiguration
        {
            Logging = new LoggingOptions
            {
                Enabled = Logging.Enabled,
                LogToConsole = Logging.LogToConsole,
                LogToFile = Logging.LogToFile,
                FilePath = Logging.FilePath
            },
            Security = new SecurityOptions
            {
                AccessCode = Security.AccessCode
            }
        };
    }
}

public sealed class LoggingOptions
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    [JsonPropertyName("logToConsole")]
    public bool LogToConsole { get; set; } = true;

    [JsonPropertyName("logToFile")]
    public bool LogToFile { get; set; } = true;

    [JsonPropertyName("filePath")]
    public string? FilePath { get; set; }
}

public sealed class SecurityOptions
{
    private const int AccessCodeLength = 10;
    private static readonly char[] CharacterPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

    [JsonPropertyName("accessCode")]
    public string? AccessCode { get; set; }

    public void EnsureAccessCode()
    {
        if (!string.IsNullOrWhiteSpace(AccessCode))
        {
            return;
        }

        var buffer = new char[AccessCodeLength];
        var random = Random.Shared;

        for (var index = 0; index < buffer.Length; index++)
        {
            buffer[index] = CharacterPool[random.Next(CharacterPool.Length)];
        }

        AccessCode = new string(buffer);
    }
}

