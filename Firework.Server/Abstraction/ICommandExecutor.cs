using Firework.Dto.Results;
using Firework.Models.Devices;

namespace Firework.Server.Abstraction;

public interface ICommandExecutor
{
    public InstructionResult ExecuteCommand(string instruction, Device device);
    public Task<string> ExecuteCommandAsync(string command);
}