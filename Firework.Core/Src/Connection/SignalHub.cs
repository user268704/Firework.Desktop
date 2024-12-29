using Firework.Abstraction.Connection;
using Firework.Abstraction.Data;
using Firework.Abstraction.HttpServer;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Abstraction.Services.NetEventService;
using Firework.Core.Logs;
using Firework.Core.Settings;
using Firework.Dto.Results;
using Firework.Mobile.Models.Models.Results;
using Firework.Models.Dto;
using Firework.Models.Events;
using Firework.Models.Instructions;
using Firework.Models.Server;
using Firework.Models.Settings;
using Microsoft.AspNetCore.SignalR;
using SQLitePCL;
using Handshake = Firework.Models.Server.Handshake;

namespace Firework.Core.Connection;

public class SignalHub : Hub, IConnectionService
{
    private readonly INetEventService _netEventService;
    private readonly IConnectionManager _connectionManager;
    private readonly IInstructionService _instructionService;
    private readonly IMacroLauncher _macroLauncher;

    private CancellationToken _cancellationToken;

    public SignalHub(INetEventService netEventService,
        IConnectionManager connectionManager,
        IDataRepository<SettingsItem> settingsRepository,
        IInstructionService instructionService,
        IMacroLauncher macroLauncher)
    {
        _netEventService = netEventService;
        _connectionManager = connectionManager;
        _instructionService = instructionService;
        _macroLauncher = macroLauncher;
    }


    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionInfo = _connectionManager.GetCurrentConnectionInfo();

        _netEventService.AddEvent(new NetworkEvent
        {
            Message = $"Соединение разорвано: {connectionInfo.ClientName}",
            EventType = NetworkEvent.TypeEvent.Disconnect,
            Date = DateTime.Now
        });

        _connectionManager.ChangeState(ConnectionState.Disconnected);

        return base.OnDisconnectedAsync(exception);
    }

    public async Task<List<InstructionResult>> Command(List<InstructionInfo> instruction)
    {
        var connectionInfo = _connectionManager.GetCurrentConnectionInfo();

        if (!connectionInfo.Equals(_connectionManager.Empty))
        {
            await InitializeConnection();
        }

        _netEventService.AddEvent(new NetworkEvent
        {
            Message = $"{instruction.First().ServiceName} ({instruction.First().ActionInfo.Name})",
            Instructions = instruction,
            EventType = NetworkEvent.TypeEvent.NewAction,
            Date = DateTime.Now,
            ClientIp = connectionInfo.ClientIp,
        });

        var result = _macroLauncher.StartRange(instruction);

        return result;
    }

    private async Task InitializeConnection()
    {
        var deviceName = _macroLauncher.Start(_instructionService.CreateInstruction("os>username"));

        var handshake = new Handshake
        {
            DeviceName = deviceName.Value,
            EndPoint = GetHost(),
        };

        var getInfoResult = await Clients.Caller.InvokeAsync<GetInfoResult>("GetInfo", handshake, _cancellationToken);

        _connectionManager.SetConnectionInfo(new ConnectionInfo
        {
            State = ConnectionState.Connected,
            ClientIp = getInfoResult.Ip,
            DateConnected = DateTime.Now,
            ClientName = getInfoResult.DeviceName,
            IsConnected = true,
        });

        _netEventService.AddEvent(new NetworkEvent
        {
            Message = $"Подключено устройство: {getInfoResult.DeviceName} ({getInfoResult.Ip})",
            EventType = NetworkEvent.TypeEvent.Connect,
            Date = DateTime.Now
        });
    }

    private string GetHost()
    {
        var instruction = _instructionService.CreateInstruction("os>getexternalipv4");
        var result = _macroLauncher.Start(instruction);

        return result.Value;
    }

    public void StartServer()
    {
        
        throw new NotImplementedException();
    }

    public void SendInstruction(string endpoint, InstructionInfo instruction)
    {
        throw new NotImplementedException();
    }

    public void SetFilter(string endpoint, IConnectionFilter filter)
    {
        throw new NotImplementedException();
    }

    public void StopServer()
    {
        throw new NotImplementedException();
    }

    public void RestartServer()
    {
        throw new NotImplementedException();
    }
}