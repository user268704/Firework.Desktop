using Firework.Dto.Instructions;

namespace Firework.Abstraction.Services;

public interface IServiceManager
{
    Dictionary<string, ServiceInfo> GetAllServices();
    IServiceBase CreateService(string name);
    IServiceBase CreateService(ServiceInfo serviceInfo);
    ServiceInfo GetServiceInfo(string serviceName);
    void AddService<T>(ServiceInfo info = null) where T : class, IServiceBase;
}