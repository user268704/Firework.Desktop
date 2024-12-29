using System.Text;
using System.Text.Json;
using Firework.Abstraction.Services;
using Firework.Core.MacroServices.Attrubutes;

namespace Firework.Core.MacroServices;

public class AppService : ServiceBase
{
    public AppService(IServiceManager serviceManager) : base(serviceManager)
    {
    }
    
    [ActionService]
    public string GetModules(string rootName)
    {
        ServiceManager serviceManager = new();

        List<string> modules = new();
        serviceManager.GetServiceInfo(rootName).ActionInfo.ForEach(item => modules.Add(item.Name));

        StringBuilder stringBuilder = new();
        
        stringBuilder.Append('(');
        var moduleNames = string.Join(',', modules);
        stringBuilder.Append(moduleNames);
        stringBuilder.Append(')');


        return stringBuilder.ToString();
    }

    [ActionService]
    public string GetServices()
    {
        ServiceManager serviceManager = new();
        
        var services = serviceManager.GetAllServices();
        var result = JsonSerializer.Serialize(services.Values, new JsonSerializerOptions()
        {
            WriteIndented = true
        });

        return result;
    }

    [ActionService]
    public string GetParams(string rootName, string moduleName)
    {
        StringBuilder stringBuilder = new();
        ServiceManager serviceManager = new();

        var service = serviceManager.GetAllServices()[rootName];
        if (service == null)
            throw new ArgumentException("There is no such root", nameof(rootName));

        var module =
            service.ActionInfo.Find(item => string.Equals(item.Name, moduleName, StringComparison.OrdinalIgnoreCase));
        if (module == null)
            throw new ArgumentException("There is no such module", nameof(moduleName));


        stringBuilder.Append('(');
        var result = string.Join(',', module.Parameters);
        stringBuilder.Append(result);
        stringBuilder.Append(')');

        return stringBuilder.ToString();
    }
}