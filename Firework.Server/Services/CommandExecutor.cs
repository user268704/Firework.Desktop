using System.Net;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Dto.Results;
using Firework.Models.Devices;
using Firework.Models.Events;
using Firework.Server.Abstraction;

namespace Firework.Server.Services;

public class CommandExecutor : ICommandExecutor
{
    private readonly IInstructionService _instructionService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMacroLauncher _macroLauncher;

    public CommandExecutor(IInstructionService instructionService,
        IAuthenticationService authenticationService,
        IMacroLauncher macroLauncher)
    {
        _instructionService = instructionService;
        _authenticationService = authenticationService;
        _macroLauncher = macroLauncher;
    }
    
    public InstructionResult ExecuteCommand(string command, Device device)
    { 
        try
        {
            var instruction = _instructionService.CreateInstruction(command);
            
            if (instruction == null)
            {
                AddEvent(new NetworkEvent
                {
                    Message = "Не валидная инструкция",
                    EventType = NetworkEvent.TypeEvent.Error,
                    Date = DateTime.Now
                }, "Не валидная инструкция");

                return new InstructionResult
                {
                    Value = "Не валидная инструкция",
                    Status = HttpStatusCode.InternalServerError
                };
            }
            
            var message = $"{instruction.ServiceName} ({instruction.ActionInfo.Name})";

            AddEvent(new NetworkEvent
            {
                Message = message,
                Instructions = [instruction],
                EventType = NetworkEvent.TypeEvent.NewAction,
                Date = DateTime.Now,
                ClientIp = _authenticationService.GetDevice(device.Hash).IP,
            }, message);

            var result = _macroLauncher.Start(instruction);

            if (result.Status == HttpStatusCode.OK)
            {
                _authenticationService.GetDevice(device.Hash).LastUpdate = DateTime.UtcNow; 
            }
            
            return result;
        }
        catch (Exception ex)
        {
            AddEvent(new NetworkEvent
            {
                Message = $"Ошибка выполнения команды: {ex.Message}",
                EventType = NetworkEvent.TypeEvent.Error,
                Date = DateTime.Now
            }, "Ошибка выполнения команды");

            return new()
                {
                    Value = $"Ошибка выполнения: {ex.Message}",
                    Status = HttpStatusCode.InternalServerError
                };
        }
    }

    public Task<string> ExecuteCommandAsync(string command)
    {
        throw new NotImplementedException();
    }
    
    private void AddEvent(NetworkEvent networkEvent, string message = "")
    {
        
    }
}