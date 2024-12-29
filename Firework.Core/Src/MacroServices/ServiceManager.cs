using System.Reflection;
using Firework.Abstraction.Services;
using Firework.Core.MacroServices.Attrubutes;
using Firework.Models.Instructions;
using Firework.Models.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Firework.Core.MacroServices;

public class ServiceManager : IServiceManager
{
    private static IServiceProvider _serviceProvider;
    private static Dictionary<string, ServiceInfo> _services;

    public IServiceProvider ServiceProvider
    {
        set
        {
            _serviceProvider = value;
        }
    }

    private static Dictionary<string, ServiceInfo> Services
    {
        get => _services ??= new();
        set => _services = value;
    }

    public Dictionary<string, ServiceInfo> GetAllServices()
    {
        return Services;
    }

    public IServiceBase CreateService(string name)
    {
        var service = Services[name];

        if (service == null)
            throw new ArgumentException("There is no such root", nameof(name));

        var result = _serviceProvider.GetRequiredService(service.Type);
        //var result = Activator.CreateInstance(service.Type) as ServiceBase;
        
        return (IServiceBase)result;
    }

    public IServiceBase CreateService(ServiceInfo serviceInfo)
    {
        var service = Services[serviceInfo.Name];

        if (service == null)
            throw new ArgumentException("There is no such root", nameof(serviceInfo));

        var result = _serviceProvider.GetService(serviceInfo.Type) as ServiceBase;
        //var result = Activator.CreateInstance(service.Type) as ServiceBase;
        
        return result;
    }

    public ServiceInfo GetServiceInfo(string serviceName)
    {
        var service = Services[serviceName.Trim()];

        if (service == null)
            throw new ArgumentException("There is no such root", nameof(serviceName));

        return service;
    }
    
    public void AddService<T>(ServiceInfo info = null) where T : class, IServiceBase
    {
        var serviceType = typeof(T);

        if (!serviceType.Name.EndsWith("Service"))
            throw new ArgumentException("The class does not contain the postfix \"Service\" in the name");

        var rootName = serviceType.Name[..serviceType.Name.IndexOf("Service", StringComparison.Ordinal)].ToLower();

        ServiceInfo service;
        if (info == null)
            service = new ServiceInfo
            {
                Name = rootName,
                Description = "",
                Title = rootName,
                Type = serviceType,
                ActionInfo = new List<ActionInfo>()
            };
        else
            service = info;
        

        var methods = serviceType.GetMethods( BindingFlags.Instance | BindingFlags.Public)
            .Where(item => item.GetCustomAttribute<ActionServiceAttribute>() != null);

        foreach (MethodInfo methodInfo in methods)
        {
            ActionInfo actionInfo;
            var attribute = methodInfo.GetCustomAttribute<ActionServiceAttribute>();

            if (!string.IsNullOrEmpty(attribute.Alias))
                actionInfo = new ActionInfo
                {
                    Parameters = new List<ActionParameterInfo>(),
                    Name = attribute.Alias.ToLower(),
                    Method = methodInfo
                };
            else
                actionInfo = new ActionInfo
                {
                    Parameters = new List<ActionParameterInfo>(),
                    Name = methodInfo.Name.ToLower(),
                    Method = methodInfo
                };

            var parameters = methodInfo.GetParameters().Select(item => new ActionParameterInfo()
            {
                Name = item.Name
            });
            
            actionInfo.Parameters.AddRange(parameters);
            service.ActionInfo.Add(actionInfo);
        }

        //_serviceCollection.AddSingleton<T>();
        
        Services.Add(service.Name, service);
    }
}