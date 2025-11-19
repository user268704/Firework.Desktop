using Firework.Abstraction.Services;
using Firework.Models.Instructions;
using Firework.Server.Abstraction;

namespace Firework.Server.Services;

public class CommandsService : ICommandsService
{
    private readonly IServiceManager _serviceManager;

    public CommandsService(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    
    public ServiceInfo GetServiceInfo(string serviceName)
    {
        return _serviceManager.GetServiceInfo(serviceName);
    }

    public Dictionary<string, ServiceInfo> GetAllServices()
    {
        return _serviceManager.GetAllServices();
    }
}