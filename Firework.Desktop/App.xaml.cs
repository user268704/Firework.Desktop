using System.Windows;
using Firework.Core.DI;
using Firework.Desktop.ViewModel;
using Firework.Desktop.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
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

        var host = new HostApplication();


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

            collection.AddNavigationViewPageProvider();
        });

        host.Build();
        host.ConfigureApplication();

        _ =  host.RunAppAsync();

    
        var mainWindow = host.ServiceProvider.GetRequiredService<MainWindow>();
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