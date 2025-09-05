using System.Windows;
using Firework.Abstraction.Data;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Abstraction.Services;
using Firework.Abstraction.Services.NetEventService;
using Firework.Core;
using Firework.Core.Data;
using Firework.Core.Instruction;
using Firework.Core.Macro;
using Firework.Core.MacroServices;
using Firework.Desktop.Services;
using Firework.Desktop.ViewModel;
using Firework.Desktop.Views.Pages;
using Firework.Abstraction.Connection;
using Firework.Models.Data;
using Firework.Models.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wpf.Ui.DependencyInjection;

namespace Firework.Desktop;

public partial class App : Application
{
    private Mutex? _mutex;
    private bool _isSingleInstance;
    private const string MutexName = "FireworkDesktopSingleInstance";
    private IHost? _host;

    public App()
    {
        _mutex = new Mutex(true, MutexName, out _isSingleInstance);

        if (!_isSingleInstance)
        {
            Current.Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        
        if (_host != null)
        {
            _host.StopAsync().Wait(5000);
            _host.Dispose();
        }

        if (_mutex != null)
        {
            _mutex.ReleaseMutex();
            _mutex.Dispose();
        }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        if (!_isSingleInstance)
        {
            return;
        }

        var hostBuilder = Host.CreateDefaultBuilder();

        hostBuilder.ConfigureServices((_, collection) =>
        {
            #region Windows

            collection.AddSingleton<MainWindow>();

            #endregion

            #region Pages

            collection.AddSingleton<MainPage>();
            collection.AddScoped<SettingsPage>();
            collection.AddScoped<EventsPage>();
            collection.AddScoped<NetworkManagerPage>();

            #endregion

            #region ViewModels

            collection.AddSingleton<SettingsViewModel>();
            collection.AddSingleton<NetworkManagerViewModel>();
            collection.AddSingleton<MainWindowViewModel>();

            #endregion

            collection.AddScoped<INetEventService, NetEventService>();
            collection.AddScoped<IMacroLauncher, MacroLauncher>();
            collection.AddScoped<IServiceManager, ServiceManager>();
            collection.AddScoped<IInstructionService, InstructionService>();
            collection.AddScoped<IDataRepository<SettingsItem>, SettingsRepository>();
            collection.AddScoped<IDataRepository<Metadata>, MetadataRepository>();
            collection.AddScoped<DbRepository, DbRepository>();
            collection.AddSingleton<IConnectionManager, DesktopConnectionManager>();
            
            collection.AddHostedService<ServerLauncherHostedService>();
            collection.AddHostedService<ServerStatusMonitorService>();
            collection.AddHostedService<ConnectionBackgroundService>();

            collection.AddNavigationViewPageProvider();
        })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        _host = hostBuilder.Build();

        // Запускаем хост в фоновом режиме
        try
        {
            _host.StartAsync().Wait();
            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("Host started successfully");
        }
        catch (Exception ex)
        {
            // Если логирование еще не настроено, выводим в консоль
            Console.WriteLine($"Error starting host: {ex.Message}");
        }

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    private void RunInTray()
    {
        throw new NotImplementedException();

        //WindowState = WindowState.Minimized;
        //ShowInTaskbar = false;

        //TrayIcon.Visibility = Visibility.Visible;
    }
}