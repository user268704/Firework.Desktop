using Firework.Abstraction.Connection;
using Firework.Abstraction.Data;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Abstraction.Services.NetEventService;
using Firework.Core.Settings;
using Firework.Dto.Dto;
using Firework.Dto.Instructions;
using Firework.Dto.Results;
using Firework.Models.Data;
using Firework.Models.Events;
using Firework.Models.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ConnectionInfo = Firework.Models.Server.ConnectionInfo;

namespace Firework.Server.Hubs;

public class SignalHub : Hub
{
    private readonly INetEventService _netEventService;
    private readonly IConnectionManager _connectionManager;
    private readonly IInstructionService _instructionService;
    private readonly IMacroLauncher _macroLauncher;
    private readonly IDataRepository<SettingsItem> _settingsRepository;
    private readonly ILogger<SignalHub> _logger;

    public SignalHub(INetEventService netEventService,
        IConnectionManager connectionManager,
        IInstructionService instructionService,
        IMacroLauncher macroLauncher,
        IDataRepository<SettingsItem> settingsRepository,
        ILogger<SignalHub> logger)
    {
        _netEventService = netEventService;
        _connectionManager = connectionManager;
        _instructionService = instructionService;
        _macroLauncher = macroLauncher;
        _settingsRepository = settingsRepository;
        _logger = logger;
    }


    public override Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var connectionInfo = _connectionManager.GetCurrentConnectionInfo();

            _netEventService.AddEvent(new NetworkEvent
            {
                Message = $"Соединение разорвано: {connectionInfo.ClientName}",
                EventType = NetworkEvent.TypeEvent.Disconnect,
                Date = DateTime.Now
            });

            _connectionManager.ChangeState(ConnectionState.Disconnected);
            
            _logger.LogInformation("Клиент отключился: {ClientName} ({ClientIp})", 
                connectionInfo.ClientName, connectionInfo.ClientIp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке отключения клиента");
        }

        return base.OnDisconnectedAsync(exception);
    }

    public override Task OnConnectedAsync()
    {
        try
        {
            _connectionManager.ChangeState(ConnectionState.Connecting);
            _logger.LogInformation("Новое подключение: {ConnectionId}", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке подключения клиента");
        }

        return base.OnConnectedAsync();
    }

    public async Task<Handshake> InitializationConnection(DeviceInfoHandshake initializationInfo)
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

            _connectionManager.SetConnectionInfo(new ConnectionInfo
            {
                State = ConnectionState.Connected,
                ClientIp = initializationInfo.Ip,
                DateConnected = DateTime.Now,
                ClientName = initializationInfo.DeviceName,
                IsConnected = true,
            });

            _netEventService.AddEvent(new NetworkEvent
            {
                Message = $"Подключено устройство: {initializationInfo.DeviceName} ({initializationInfo.Ip})",
                EventType = NetworkEvent.TypeEvent.Connect,
                Date = DateTime.Now
            });

            var instructionGetUsername = _instructionService.CreateInstruction("os>username");
            var deviceName = _macroLauncher.Start(instructionGetUsername);

            var handshake = new Handshake
            {
                DeviceName = deviceName.Value,
                EndPoint = GetHost(),
            };

            _logger.LogInformation("Инициализация соединения завершена для {DeviceName} ({Ip})", 
                initializationInfo.DeviceName, initializationInfo.Ip);

            return handshake;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка инициализации соединения для {DeviceName}", 
                initializationInfo?.DeviceName ?? "Unknown");

            _netEventService.AddEvent(new NetworkEvent
            {
                Message = $"Ошибка инициализации соединения: {ex.Message}",
                EventType = NetworkEvent.TypeEvent.Error,
                Date = DateTime.Now
            });

            throw;
        }
    }

    public async Task<List<InstructionResult>> Command(List<InstructionInfo> instruction)
    {
        try
        {
            if (instruction == null || !instruction.Any())
            {
                _logger.LogWarning("Получен пустой список инструкций");
                return new List<InstructionResult>
                {
                    new()
                    {
                        Value = "Список инструкций пуст или null",
                        Status = StatusCode.Error
                    }
                };
            }

            // Валидация каждой инструкции
            foreach (var inst in instruction)
            {
                if (string.IsNullOrWhiteSpace(inst.ServiceName))
                {
                    return new List<InstructionResult>
                    {
                        new()
                        {
                            Value = "Имя сервиса не может быть пустым",
                            Status = StatusCode.Error
                        }
                    };
                }

                if (inst.ActionInfo == null || string.IsNullOrWhiteSpace(inst.ActionInfo.Name))
                {
                    return new List<InstructionResult>
                    {
                        new()
                        {
                            Value = "Информация о действии не может быть пустой",
                            Status = StatusCode.Error
                        }
                    };
                }
            }

            var connectionInfo = _connectionManager.GetCurrentConnectionInfo();

            if (connectionInfo.State is not ConnectionState.Connected)
            {
                _logger.LogWarning("Попытка выполнить команду без установленного соединения");
                return new List<InstructionResult>
                {
                    new()
                    {
                        Value = "Соединение не установлено",
                        Status = StatusCode.Error
                    }
                };
            }

            _netEventService.AddEvent(new NetworkEvent
            {
                Message = $"{instruction.First().ServiceName} ({instruction.First().ActionInfo.Name})",
                Instructions = instruction,
                EventType = NetworkEvent.TypeEvent.NewAction,
                Date = DateTime.Now,
                ClientIp = connectionInfo.ClientIp,
            });

            _logger.LogDebug("Выполнение команды: {ServiceName}.{ActionName}", 
                instruction.First().ServiceName, instruction.First().ActionInfo.Name);

            var result = _macroLauncher.StartRange(instruction);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка выполнения команды");

            _netEventService.AddEvent(new NetworkEvent
            {
                Message = $"Ошибка выполнения команды: {ex.Message}",
                EventType = NetworkEvent.TypeEvent.Error,
                Date = DateTime.Now
            });

            return new List<InstructionResult>
            {
                new()
                {
                    Value = $"Ошибка выполнения: {ex.Message}",
                    Status = StatusCode.Error
                }
            };
        }
    }

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