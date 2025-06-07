using Firework.Abstraction.Data;
using Firework.Models.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Firework.Core.DI;

public class HostApplication
{
    private readonly IDataRepository<SettingsItem> _settingsRepository;

    public IServiceProvider ServiceProvider => _webApplication.Services;

    private WebApplicationBuilder _webBuilder;
    private WebApplication _webApplication;

    public HostApplication()
    {
        _webBuilder = WebApplication.CreateBuilder();
    }

    public void ConfigureServices(Action<HostBuilderContext, IServiceCollection> configure)
    {
        _webBuilder.Host.ConfigureServices(configure);
    }

    public void Build()
    {
        _webApplication = _webBuilder.Build();
    }

    public async Task RunAppAsync()
    {
        await _webApplication.RunAsync();
    }

    public void ConfigureApplication()
    {
        _webApplication.UseRouting();

        /*
        var host = _settingsRepository
            .GetAll()
            .FirstOrDefault(x => x.UniqueKey == nameof(SettingsName.LocalHost));
            */

        /*
        var port = _settingsRepository
            .GetAll()
            .FirstOrDefault(x => x.UniqueKey == nameof(SettingsName.LocalPort));
            */


        string host = "*";
        int port = 5062;

        _webApplication.Urls.Add($"http://{host}:{port}");
        //_webApplication.MapHub<SignalHub>("/signal");
        _webApplication.MapGet("/index", () => "Hello World!");
    }
}