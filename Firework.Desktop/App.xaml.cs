using System.Windows;
using Firework.Abstraction.Connection;
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
using Firework.Models.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.DependencyInjection;

namespace Firework.Desktop;

public partial class App : Application
{
    private Mutex? _mutex;
    private bool _isSingleInstance;
    private const string MutexName = "FireworkDesktopSingleInstance";

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

        var host = Host.CreateDefaultBuilder();


        host.ConfigureServices((_, collection) =>
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
            collection.AddScoped<DbRepository, DbRepository>();
            
            
            collection.AddHostedService<ServerLauncherHostedService>();

            collection.AddNavigationViewPageProvider();
        });
        
        var app = host.Build();

    
        var mainWindow = app.Services.GetRequiredService<MainWindow>();
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