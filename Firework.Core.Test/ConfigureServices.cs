using Microsoft.Extensions.DependencyInjection;

namespace Firework.Core.Test;

public class ConfigureServices
{
    public IServiceProvider ServiceProvider { get; }
    public ConfigureServices()
    {
        /*
        // Создаем коллекцию сервисов для DI
        var serviceCollection = new HostApplication();
        
        // Регистрируем реальные зависимости
        serviceCollection.Services.AddSingleton<IServiceManager, ServiceManager>();
        serviceCollection.Services.AddSingleton<IInstructionService, InstructionService>();
        serviceCollection.Services.AddSingleton<IMacroLauncher, MacroLauncher>();

        serviceCollection.Services.AddSingleton<TestService>();

        var host = serviceCollection.Build();
        
        // Создаем ServiceProvider (контейнер для DI)
        ServiceProvider = host.Services;
        
        // Получаем экземпляр тестируемого сервиса через DI
        var serviceManager = ServiceProvider.GetRequiredService<IServiceManager>();

        AddInstructionServices(serviceManager);
    */
    }

    /*
    private void AddInstructionServices(IServiceManager serviceManager)
    {
        var services = serviceManager.GetAllServices();

        if (!services.ContainsKey("task")) 
            serviceManager.AddService<TaskService>();

        if (!services.ContainsKey("test"))
            serviceManager.AddService<TestService>();
    }
*/
}