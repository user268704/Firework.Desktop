using Firework.Abstraction.Connection;
using Firework.Abstraction.Data;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Core.Settings;
using Firework.Dto.Instructions;
using Firework.Dto.Results;
using Firework.Models.Data;
using Firework.Models.Server;
using Microsoft.AspNetCore.SignalR;
using ConnectionInfo = Firework.Models.Server.ConnectionInfo;

namespace Firework.Server.Hubs;

public class SignalHub : Hub
{
    private readonly IConnectionManager _connectionManager;
    private readonly IInstructionService _instructionService;
    private readonly IMacroLauncher _macroLauncher;
    private readonly IDataRepository<SettingsItem> _settingsRepository;
    private readonly ILogger<SignalHub> _logger;

    public SignalHub(IConnectionManager connectionManager,
        IInstructionService instructionService,
        IMacroLauncher macroLauncher,
        IDataRepository<SettingsItem> settingsRepository,
        ILogger<SignalHub> logger)
    {
        _connectionManager = connectionManager;
        _instructionService = instructionService;
        _macroLauncher = macroLauncher;
        _settingsRepository = settingsRepository;
        _logger = logger;
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public ValueTask<Handshake> InitializationConnection(DeviceInfoHandshake initializationInfo)
    {
        try
        {
            if (initializationInfo == null)
            {
                throw new ArgumentNullException(nameof(initializationInfo));
            }

            if (string.IsNullOrWhiteSpace(initializationInfo.DeviceName))
            {
                throw new ArgumentException("Имя устройства не может быть пустым", nameof(initializationInfo.DeviceName));
            }

            if (string.IsNullOrWhiteSpace(initializationInfo.Ip))
            {
                throw new ArgumentException("IP адрес не может быть пустым", nameof(initializationInfo.Ip));
            }

            _connectionManager.ChangeState(ConnectionState.Connected);

            // Авторизуем устройство
            //var deviceInfo = _deviceAuthorizationService.AuthorizeDevice(Context.ConnectionId, initializationInfo);

            _connectionManager.SetConnectionInfo(new ConnectionInfo
            {
                State = ConnectionState.Connected,
                ClientIp = initializationInfo.Ip,
                DateConnected = DateTime.Now,
                ClientName = initializationInfo.DeviceName,
                IsConnected = true,
            });

            /*
            AddEventAndSendMessageToPipe(new NetworkEvent
            {
                Message = $"Подключено устройство: {initializationInfo.DeviceName} ({initializationInfo.Ip})",
                EventType = NetworkEvent.TypeEvent.Connect,
                Date = DateTime.Now
            }, logLevel: LogLevel.Information, 
                logMessage: "Инициализация соединения завершена для {DeviceName} ({Ip})", 
                logArgs: new object[] { initializationInfo.DeviceName, initializationInfo.Ip });
                */

            var instructionGetUsername = _instructionService.CreateInstruction("os>username");
            var deviceName = _macroLauncher.Start(instructionGetUsername);

            var handshake = new Handshake
            {
                DeviceName = deviceName.Value,
                EndPoint = GetHost(),
            };

            return ValueTask.FromResult(handshake);
        }
        catch (Exception ex)
        {
            /*
            AddEventAndSendMessageToPipe(new NetworkEvent
            {
                Message = $"Ошибка инициализации соединения: {ex.Message}",
                EventType = NetworkEvent.TypeEvent.Error,
                Date = DateTime.Now
            }, logLevel: LogLevel.Error, 
                logMessage: "Ошибка инициализации соединения для {DeviceName}", 
                logArgs: new object[] { initializationInfo?.DeviceName ?? "Unknown" },
                logException: ex);
                */

            throw;
        }
    }

    public async Task<List<InstructionResult>> Command(List<InstructionInfoDto> instruction)
    {
        throw new NotImplementedException();
    }

    /*
    private void AddEventAndSendMessageToPipe(NetworkEvent networkEvent, string message = "", 
        LogLevel logLevel = LogLevel.Information, string logMessage = null, 
        object[] logArgs = null, Exception logException = null)
    {
        _netEventService.AddEvent(networkEvent);
        
        if (!string.IsNullOrEmpty(logMessage))
        {
            if (logException != null)
            {
                _logger.Log(logLevel, logException, logMessage, logArgs ?? Array.Empty<object>());
            }
            else
            {
                _logger.Log(logLevel, logMessage, logArgs ?? Array.Empty<object>());
            }
        }
    }
    */

    private string GetHost()
    {
        try
        {
            var instruction = _instructionService.CreateInstruction("os>getexternalipv4");
            var result = _macroLauncher.Start(instruction);

            var port = _settingsRepository.FindBy(x => x.UniqueKey == SettingsDefault.Names.LocalPort);

            if (port == null)
            {
                throw new InvalidOperationException("Порт не найден в настройках");
            }

            var host = result.Value + ":" + port.Value;
            _logger.LogDebug("Получен хост: {Host}", host);
            return host;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка получения хоста");
            return "localhost:5000"; // Fallback значение
        }
    }
}