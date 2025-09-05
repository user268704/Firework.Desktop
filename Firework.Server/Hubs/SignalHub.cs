using System.Text;
using Firework.Abstraction.Connection;
using Firework.Abstraction.Data;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Abstraction.Services.NetEventService;
using Firework.Core.Services;
using Firework.Core.Services.PipeBroker;
using Firework.Core.Settings;
using Firework.Dto.Dto;
using Firework.Dto.Instructions;
using Firework.Dto.Results;
using Firework.Models.Data;
using Firework.Models.Events;
using Firework.Models.Metadata;
using Firework.Models.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ConnectionInfo = Firework.Models.Server.ConnectionInfo;

namespace Firework.Server.Hubs;

public class SignalHub : Hub
{
    private readonly INetEventService _netEventService;
    private readonly IConnectionManager _connectionManager;
    private readonly IDeviceConnectionService _deviceConnectionService;
    private readonly IDeviceAuthorizationService _deviceAuthorizationService;
    private readonly IInstructionService _instructionService;
    private readonly IMacroLauncher _macroLauncher;
    private readonly IDataRepository<SettingsItem> _settingsRepository;
    private readonly ILogger<SignalHub> _logger;
    private PipeService _pipeService;

    public SignalHub(INetEventService netEventService,
        IConnectionManager connectionManager,
        IDeviceConnectionService deviceConnectionService,
        IDeviceAuthorizationService deviceAuthorizationService,
        IInstructionService instructionService,
        IMacroLauncher macroLauncher,
        IDataRepository<SettingsItem> settingsRepository,
        IDataRepository<Metadata> metadataRepository,
        ILogger<SignalHub> logger)
    {
        _netEventService = netEventService;
        _connectionManager = connectionManager;
        _deviceConnectionService = deviceConnectionService;
        _deviceAuthorizationService = deviceAuthorizationService;
        _instructionService = instructionService;
        _macroLauncher = macroLauncher;
        _settingsRepository = settingsRepository;
        _logger = logger;

        var pipeName = metadataRepository.FindBy(metadata => metadata.Name == SettingsDefault.Names.ServerPipeName);
        
        _pipeService = new PipeServiceBuilder(pipeName.Value)
            .OnConnected(OnPipeConnected)
            .WithLogging(_logger)
            .WithMessageHandler(PipeMessageHandler)
            .Build();
        
        _pipeService.Start();
    }

    private void PipeMessageHandler(byte[] message)
    {
        Console.WriteLine(Encoding.UTF8.GetString(message));
    }
    
    private void OnPipeConnected()
    {
        Console.WriteLine("Подключено к pipe серверу");
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var deviceInfo = _deviceAuthorizationService.GetAuthorizedDevice(Context.ConnectionId);
            
            if (deviceInfo != null)
            {
                AddEventAndSendMessageToPipe(new NetworkEvent
                {
                    Message = $"Соединение разорвано: {deviceInfo.DeviceName}",
                    EventType = NetworkEvent.TypeEvent.Disconnect,
                    Date = DateTime.Now
                }, logLevel: LogLevel.Information, logMessage: "Клиент отключился: {DeviceName} ({IpAddress})", 
                    logArgs: new object[] { deviceInfo.DeviceName, deviceInfo.IpAddress });

                _deviceAuthorizationService.RevokeAuthorization(Context.ConnectionId);
            }
            else
            {
                var connectionInfo = _connectionManager.GetCurrentConnectionInfo();
                AddEventAndSendMessageToPipe(new NetworkEvent
                {
                    Message = $"Соединение разорвано: {connectionInfo.ClientName}",
                    EventType = NetworkEvent.TypeEvent.Disconnect,
                    Date = DateTime.Now
                }, logLevel: LogLevel.Information, logMessage: "Клиент отключился: {ClientName} ({ClientIp})", 
                    logArgs: new object[] { connectionInfo.ClientName, connectionInfo.ClientIp });

                _connectionManager.ChangeState(ConnectionState.Disconnected);
            }
        }
        catch (Exception ex)
        {
            AddEventAndSendMessageToPipe(new NetworkEvent
            {
                Message = $"Ошибка при обработке отключения клиента: {ex.Message}",
                EventType = NetworkEvent.TypeEvent.Error,
                Date = DateTime.Now
            }, logLevel: LogLevel.Error, logMessage: "Ошибка при обработке отключения клиента", 
                logException: ex);
        }

        return base.OnDisconnectedAsync(exception);
    }

    public override Task OnConnectedAsync()
    {
        try
        {
            _connectionManager.ChangeState(ConnectionState.Connecting);
            AddEventAndSendMessageToPipe(new NetworkEvent
            {
                Message = $"Новое подключение: {Context.ConnectionId}",
                EventType = NetworkEvent.TypeEvent.Connect,
                Date = DateTime.Now
            }, logLevel: LogLevel.Information, logMessage: "Новое подключение: {ConnectionId}", 
                logArgs: new object[] { Context.ConnectionId });
        }
        catch (Exception ex)
        {
            AddEventAndSendMessageToPipe(new NetworkEvent
            {
                Message = $"Ошибка при обработке подключения клиента: {ex.Message}",
                EventType = NetworkEvent.TypeEvent.Error,
                Date = DateTime.Now
            }, logLevel: LogLevel.Error, logMessage: "Ошибка при обработке подключения клиента", 
                logException: ex);
        }

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

            AddEventAndSendMessageToPipe(new NetworkEvent
            {
                Message = $"Подключено устройство: {initializationInfo.DeviceName} ({initializationInfo.Ip})",
                EventType = NetworkEvent.TypeEvent.Connect,
                Date = DateTime.Now
            }, logLevel: LogLevel.Information, 
                logMessage: "Инициализация соединения завершена для {DeviceName} ({Ip})", 
                logArgs: new object[] { initializationInfo.DeviceName, initializationInfo.Ip });

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
            AddEventAndSendMessageToPipe(new NetworkEvent
            {
                Message = $"Ошибка инициализации соединения: {ex.Message}",
                EventType = NetworkEvent.TypeEvent.Error,
                Date = DateTime.Now
            }, logLevel: LogLevel.Error, 
                logMessage: "Ошибка инициализации соединения для {DeviceName}", 
                logArgs: new object[] { initializationInfo?.DeviceName ?? "Unknown" },
                logException: ex);

            throw;
        }
    }

    public async Task<List<InstructionResult>> Command(List<InstructionInfoDto> instruction)
    {
        try
        {
            if (instruction == null || !instruction.Any())
            {
                AddEventAndSendMessageToPipe(new NetworkEvent
                {
                    Message = "Список инструкций пуст или null",
                    EventType = NetworkEvent.TypeEvent.Error,
                    Date = DateTime.Now
                }, logLevel: LogLevel.Warning, logMessage: "Получен пустой список инструкций");
                
                return new List<InstructionResult>
                {
                    new()
                    {
                        Value = "Список инструкций пуст или null",
                        Status = StatusCode.Error
                    }
                };
            }
            
            // Проверка авторизации - устройство должно быть авторизовано
            var deviceInfo = _deviceAuthorizationService.GetAuthorizedDevice(Context.ConnectionId);
            if (deviceInfo == null)
            {
                AddEventAndSendMessageToPipe(new NetworkEvent
                {
                    Message = "Устройство не авторизовано. Необходимо выполнить инициализацию соединения.",
                    EventType = NetworkEvent.TypeEvent.Error,
                    Date = DateTime.Now
                }, logLevel: LogLevel.Warning, 
                    logMessage: "Попытка выполнить команду без авторизации. ConnectionId: {ConnectionId}", 
                    logArgs: new object[] { Context.ConnectionId });
                
                return new List<InstructionResult>
                {
                    new()
                    {
                        Value = "Устройство не авторизовано. Необходимо выполнить инициализацию соединения.",
                        Status = StatusCode.Error
                    }
                };
            }

            // Проверка валидности и прав доступа к инструкциям
            foreach (var inst in instruction)
            {
                if (string.IsNullOrWhiteSpace(inst.ServiceName))
                {
                    AddEventAndSendMessageToPipe(new NetworkEvent
                    {
                        Message = "Имя сервиса не может быть пустым",
                        EventType = NetworkEvent.TypeEvent.Error,
                        Date = DateTime.Now
                    }, logLevel: LogLevel.Error, logMessage: "Имя сервиса не может быть пустым");
                    
                    return
                    [
                        new()
                        {
                            Value = "Имя сервиса не может быть пустым",
                            Status = StatusCode.Error
                        }
                    ];
                }

                if (string.IsNullOrWhiteSpace(inst.ActionName))
                {
                    AddEventAndSendMessageToPipe(new NetworkEvent
                    {
                        Message = "Имя действия не может быть пустым",
                        EventType = NetworkEvent.TypeEvent.Error,
                        Date = DateTime.Now
                    }, logLevel: LogLevel.Error, logMessage: "Имя действия не может быть пустым");
                    
                    return
                    [
                        new()
                        {
                            Value = "Имя действия не может быть пустым",
                            Status = StatusCode.Error
                        }
                    ];
                }

                // Проверка прав доступа к сервису
                if (!_deviceAuthorizationService.HasServiceAccess(Context.ConnectionId, inst.ServiceName))
                {
                    AddEventAndSendMessageToPipe(new NetworkEvent
                    {
                        Message = $"Доступ к сервису '{inst.ServiceName}' запрещен",
                        EventType = NetworkEvent.TypeEvent.Error,
                        Date = DateTime.Now
                    }, logLevel: LogLevel.Error, logMessage: $"Доступ к сервису '{inst.ServiceName}' запрещен");
                    
                    return new List<InstructionResult>
                    {
                        new()
                        {
                            Value = $"Доступ к сервису '{inst.ServiceName}' запрещен",
                            Status = StatusCode.Error
                        }
                    };
                }

                // Проверка прав доступа к действию
                if (!_deviceAuthorizationService.HasActionAccess(Context.ConnectionId, inst.ServiceName, inst.ActionName))
                {
                    AddEventAndSendMessageToPipe(new NetworkEvent
                    {
                        Message = $"Доступ к действию '{inst.ActionName}' в сервисе '{inst.ServiceName}' запрещен",
                        EventType = NetworkEvent.TypeEvent.Error,
                        Date = DateTime.Now
                    }, logLevel: LogLevel.Error, 
                        logMessage: $"Доступ к действию '{inst.ActionName}' в сервисе '{inst.ServiceName}' запрещен");
                    
                    return new List<InstructionResult>
                    {
                        new()
                        {
                            Value = $"Доступ к действию '{inst.ActionName}' в сервисе '{inst.ServiceName}' запрещен",
                            Status = StatusCode.Error
                        }
                    };
                }
            }

            // Маппинг из DTO в модель
            var mappedInstructions = InstructionMapper.MapToInstructionInfoList(instruction);

            // Обновляем время последней активности устройства
            _deviceConnectionService.UpdateDevice(Context.ConnectionId, device => 
            {
                device.LastActivity = DateTime.Now;
            });

            var message = $"{instruction.First().ServiceName} ({instruction.First().ActionName})";
            
            AddEventAndSendMessageToPipe(new NetworkEvent
            {
                Message = message,
                Instructions = mappedInstructions,
                EventType = NetworkEvent.TypeEvent.NewAction,
                Date = DateTime.Now,
                ClientIp = deviceInfo.IpAddress,
            }, message, LogLevel.Debug, 
                "Выполнение команды от устройства {DeviceName}: {ServiceName}.{ActionName}", 
                new object[] { deviceInfo.DeviceName, instruction.First().ServiceName, instruction.First().ActionName });

            var result = _macroLauncher.StartRange(mappedInstructions);
            return result;
        }
        catch (Exception ex)
        {
            AddEventAndSendMessageToPipe(new NetworkEvent
            {
                Message = $"Ошибка выполнения команды: {ex.Message}",
                EventType = NetworkEvent.TypeEvent.Error,
                Date = DateTime.Now
            }, "Ошибка выполнения команды", LogLevel.Error, 
                "Ошибка выполнения: {ErrorMessage}", 
                new object[] { ex.Message });

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

    private void AddEventAndSendMessageToPipe(NetworkEvent networkEvent, string message = "", 
        LogLevel logLevel = LogLevel.Information, string logMessage = null, 
        object[] logArgs = null, Exception logException = null)
    {
        _netEventService.AddEvent(networkEvent);

        if (!string.IsNullOrEmpty(message))
        {
            _pipeService.SendMessage(message);
        }

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