using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Firework.Desktop.Services;

public class ServerLauncherHostedService : IHostedService
{
    private readonly ILogger<ServerLauncherHostedService> _logger;
    private readonly string _serverPath = "Firework.Server.exe";
    private static Process? _serverProcess;
    
    public ServerLauncherHostedService(ILogger<ServerLauncherHostedService> logger)
    {
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = _serverPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        _serverProcess = Process.Start(processStartInfo);
        
        _logger.LogInformation("Server process started with ID: {ProcessId}", _serverProcess.Id);

        _serverProcess.OutputDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                _logger.LogInformation("{Output}", args.Data);
            }
        };
        
        _serverProcess.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                _logger.LogError("{Error}", args.Data);
            }
        };
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        KillServer();
    }
    
    private void KillServer()
    {
        if (_serverProcess is { HasExited: false })
        {
            _logger.LogInformation("Killing server process with ID: {ProcessId}", _serverProcess.Id);
            
            _serverProcess.Kill();
            _serverProcess.Dispose();
            _serverProcess = null;
        }
    }

    private Dictionary<string, string> GetConfig()
    {
        var dictionary = new Dictionary<string, string>
        { };
        
        return dictionary;
    }
    
    
    private bool IsServerRunning()
    {
        return _serverProcess is { HasExited: false };
    }
}