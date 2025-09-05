using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Firework.Abstraction.Connection;
using Firework.Models.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Firework.Desktop.Services;

public class ServerStatusMonitorService : BackgroundService
{
    private readonly ILogger<ServerStatusMonitorService> _logger;
    private readonly IConnectionManager _connectionManager;
    private readonly HttpClient _httpClient;
    private readonly string _serverUrl = "http://localhost:5000";

    public ServerStatusMonitorService(
        ILogger<ServerStatusMonitorService> logger,
        IConnectionManager connectionManager)
    {
        _logger = logger;
        _connectionManager = connectionManager;
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(5);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ServerStatusMonitorService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckServerStatus();
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Проверяем каждые 10 секунд
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking server status");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // При ошибке ждем дольше
            }
        }
    }

    private async Task CheckServerStatus()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_serverUrl}/health");
            
            if (response.IsSuccessStatusCode)
            {
                var currentInfo = _connectionManager.GetCurrentConnectionInfo();
                if (!currentInfo.IsConnected)
                {
                    _connectionManager.SetConnectionInfo(new ConnectionInfo
                    {
                        ClientName = "ожидание подключения",
                        ClientIp = "0.0.0.0",
                        IsConnected = false,
                        State = ConnectionState.NotConnected,
                        DateConnected = DateTime.MinValue
                    });
                }
            }
        }
        catch (HttpRequestException)
        {
            // Сервер недоступен
            _connectionManager.SetConnectionInfo(new ConnectionInfo
            {
                ClientName = "сервер недоступен",
                ClientIp = "0.0.0.0",
                IsConnected = false,
                State = ConnectionState.NotConnected,
                DateConnected = DateTime.MinValue
            });
        }
    }

    public override void Dispose()
    {
        _httpClient?.Dispose();
        base.Dispose();
    }
}
