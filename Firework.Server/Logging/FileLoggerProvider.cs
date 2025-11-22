using Microsoft.Extensions.Logging;

namespace Firework.Server.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;
    private readonly object _syncRoot = new();

    public FileLoggerProvider(string filePath)
    {
        _filePath = filePath;
        var directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_filePath, _syncRoot);
    }

    public void Dispose()
    {
    }

    private sealed class FileLogger : ILogger
    {
        private readonly string _filePath;
        private readonly object _syncRoot;

        public FileLogger(string filePath, object syncRoot)
        {
            _filePath = filePath;
            _syncRoot = syncRoot;
        }

        public IDisposable? BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (formatter is null)
            {
                return;
            }

            var message = $"{DateTime.UtcNow:O} [{logLevel}] {formatter(state, exception)}";
            var exceptionMessage = exception == null ? string.Empty : Environment.NewLine + exception;

            lock (_syncRoot)
            {
                File.AppendAllText(_filePath, message + exceptionMessage + Environment.NewLine);
            }
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}

