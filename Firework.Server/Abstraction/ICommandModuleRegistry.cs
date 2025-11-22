using Firework.Server.Dto.Commands;
using Firework.Server.Models.Commands;
using FluentResults;

namespace Firework.Server.Abstraction;

public interface ICommandModuleRegistry
{
    Result<CommandModuleDescriptor> GetModule(string moduleName);
    IEnumerable<CommandDescriptorDto> GetCommandDescriptors();
}

