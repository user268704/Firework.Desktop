using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Firework.Core.Services.PipeBroker;
using System.Text;
using System.Text.Json;
using Firework.Abstraction.Data;
using Firework.Desktop.Services.Settings;
using Firework.Models.Metadata;

namespace Firework.Desktop.Services;

public class ConnectionBackgroundService : BackgroundService
{
    private readonly ILogger<ConnectionBackgroundService> _logger;
    private PipeService? _pipeService;
    private readonly string _pipeName;

    public ConnectionBackgroundService(ILogger<ConnectionBackgroundService> logger, IDataRepository<Metadata> metadataRepository)
    {
        _logger = logger;
        _pipeName = metadataRepository.FindBy(x => x.Name == SettingsDefault.Names.ServerPipeName).Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await InitializePipeServiceAsync(stoppingToken);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_pipeService != null)
                    {
                        _logger.LogInformation("Запуск PipeService клиента...");
                        _pipeService.Start();
                    }
                    else
                    {
                        _logger.LogError("PipeService не инициализирован");
                        await Task.Delay(5000, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при работе с pipe сервисом");
                    await Task.Delay(5000, stoppingToken); // Пауза перед повторной попыткой
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("ConnectionBackgroundService остановлен");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Критическая ошибка в ConnectionBackgroundService");
        }
    }

    private Task InitializePipeServiceAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            _pipeService = new PipeServiceBuilder(_pipeName)
                .AsClient()
                .WithLogging(_logger)
                .WithMessageHandler(HandleIncomingMessage)
                .OnConnected(HandleConnection)
                .Build();
                
            _logger.LogInformation("PipeService инициализирован как клиент");
        }, cancellationToken);
    }

    private void HandleConnection()
    {
        _logger.LogInformation("Подключение к серверу установлено");
    }

    private void HandleIncomingMessage(byte[] messageBytes)
    {
        try
        {
            var decodedMessage = DecodeMessage(messageBytes);
            ProcessMessage(decodedMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке входящего сообщения");
        }
    }

    private string DecodeMessage(byte[] messageBytes)
    {
        if (messageBytes == null || messageBytes.Length == 0)
        {
            _logger.LogWarning("Получено пустое сообщение");
            return string.Empty;
        }

        // Удаляем нулевые байты в конце (если есть)
        var actualLength = Array.FindLastIndex(messageBytes, b => b != 0) + 1;
        if (actualLength <= 0) return string.Empty;

        var trimmedBytes = new byte[actualLength];
        Array.Copy(messageBytes, trimmedBytes, actualLength);

        var decodedString = Encoding.UTF8.GetString(trimmedBytes);
        _logger.LogDebug("Декодировано сообщение: {Message}", decodedString);
        
        return decodedString;
    }

    private void ProcessMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            _logger.LogWarning("Получено пустое сообщение для обработки");
            return;
        }

        _logger.LogInformation("Обработка сообщения: {Message}", message);

        try
        {
            // Попытка парсинга как JSON
            if (TryParseAsJson(message, out var jsonObject) && jsonObject != null)
            {
                ProcessJsonMessage(jsonObject);
            }
            else
            {
                ProcessPlainTextMessage(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке сообщения: {Message}", message);
        }
    }

    private bool TryParseAsJson(string message, out JsonDocument? jsonDocument)
    {
        jsonDocument = null;
        try
        {
            jsonDocument = JsonDocument.Parse(message);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private void ProcessJsonMessage(JsonDocument jsonDocument)
    {
        // Заглушка для обработки JSON сообщений
        _logger.LogInformation("Обработка JSON сообщения...");

        if (jsonDocument.RootElement.TryGetProperty("type", out var typeProperty))
        {
            var messageType = typeProperty.GetString();
            _logger.LogInformation("Тип сообщения: {MessageType}", messageType);

            switch (messageType?.ToLower())
            {
                case "command":
                    HandleCommandMessage(jsonDocument);
                    break;
                case "status":
                    HandleStatusMessage(jsonDocument);
                    break;
                case "data":
                    HandleDataMessage(jsonDocument);
                    break;
                default:
                    _logger.LogWarning("Неизвестный тип сообщения: {MessageType}", messageType);
                    break;
            }
        }
        else
        {
            _logger.LogInformation("JSON сообщение без типа, выполняется общая обработка");
            HandleGenericJsonMessage(jsonDocument);
        }
    }

    private void ProcessPlainTextMessage(string message)
    {
        // Заглушка для обработки текстовых сообщений
        _logger.LogInformation("Обработка текстового сообщения: {Message}", message);
        
        // Здесь можно добавить логику для обработки команд в текстовом формате
        if (message.StartsWith("CMD:", StringComparison.OrdinalIgnoreCase))
        {
            var command = message.Substring(4).Trim();
            _logger.LogInformation("Получена команда: {Command}", command);
            // Заглушка обработки команды
        }
        else if (message.StartsWith("DATA:", StringComparison.OrdinalIgnoreCase))
        {
            var data = message.Substring(5).Trim();
            _logger.LogInformation("Получены данные: {Data}", data);
            // Заглушка обработки данных
        }
        else
        {
            _logger.LogInformation("Обычное сообщение: {Message}", message);
            // Заглушка общей обработки
        }
    }

    private void HandleCommandMessage(JsonDocument jsonDocument)
    {
        _logger.LogInformation("Обработка команды из JSON...");
        // Заглушка для обработки команд
        
        if (jsonDocument.RootElement.TryGetProperty("action", out var actionProperty))
        {
            var action = actionProperty.GetString();
            _logger.LogInformation("Выполнение действия: {Action}", action);
            // Заглушка выполнения действия
        }
    }

    private void HandleStatusMessage(JsonDocument jsonDocument)
    {
        _logger.LogInformation("Обработка статуса из JSON...");
        // Заглушка для обработки статуса
        
        if (jsonDocument.RootElement.TryGetProperty("status", out var statusProperty))
        {
            var status = statusProperty.GetString();
            _logger.LogInformation("Получен статус: {Status}", status);
            // Заглушка обработки статуса
        }
    }

    private void HandleDataMessage(JsonDocument jsonDocument)
    {
        _logger.LogInformation("Обработка данных из JSON...");
        // Заглушка для обработки данных
        
        if (jsonDocument.RootElement.TryGetProperty("payload", out var payloadProperty))
        {
            var payload = payloadProperty.ToString();
            _logger.LogInformation("Получена полезная нагрузка: {Payload}", payload);
            // Заглушка обработки полезной нагрузки
        }
    }

    private void HandleGenericJsonMessage(JsonDocument jsonDocument)
    {
        _logger.LogInformation("Обработка общего JSON сообщения...");
        // Заглушка для обработки произвольных JSON сообщений
        
        var jsonString = jsonDocument.RootElement.ToString();
        _logger.LogDebug("Содержимое JSON: {Json}", jsonString);
        // Заглушка общей обработки JSON
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Остановка ConnectionBackgroundService...");
        
        try
        {
            _pipeService?.Dispose();
            _pipeService = null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при освобождении ресурсов");
        }
        
        await base.StopAsync(cancellationToken);
    }
}