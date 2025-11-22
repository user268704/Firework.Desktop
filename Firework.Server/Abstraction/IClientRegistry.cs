using Firework.Server.Dto.Devices;
using Firework.Server.Models.Clients;
using FluentResults;

namespace Firework.Server.Abstraction;

public interface IClientRegistry
{
    Result<RegisteredClient> Register(DevicePayloadDto payload, string ipAddress);
    Result<RegisteredClient> GetByToken(string token);
    Result<IReadOnlyCollection<RegisteredClient>> GetAll();
    void Touch(string token);
}

