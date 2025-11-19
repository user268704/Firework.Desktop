using Firework.Dto.Instructions;
using Firework.Models.Instructions;

namespace Firework.Server.Abstraction;

public interface ICommandsService
{
    ServiceInfo GetServiceInfo(string serviceName);
    Dictionary<string, ServiceInfo> GetAllServices();
}