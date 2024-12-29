using System;
using System.Threading;
using System.Windows;
using Firework.Abstraction.Data;
using Firework.Abstraction.HttpServer;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Abstraction.Services;
using Firework.Abstraction.Services.FileService;
using Firework.Abstraction.Services.NetEventService;
using Firework.Core;
using Firework.Core.Connection;
using Firework.Core.Data;
using Firework.Core.DI;
using Firework.Core.Instruction;
using Firework.Core.Macro;
using Firework.Core.MacroServices;
using Firework.Core.Services;
using Firework.Desktop.Services;
using Firework.Desktop.ViewModel;
using Firework.Desktop.Views.Pages;
using Firework.Models.Metadata;
using Firework.Models.Settings;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Application = System.Windows.Application;

namespace Firework.Desktop
{
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

            ServiceManager serviceManager = new();

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

                #region Services

                collection.AddScoped<IInstructionService, InstructionService>();
                collection.AddScoped<IPageService, PageService>();
                collection.AddScoped<IFileService, FileService>();
                collection.AddSingleton<INetEventService, NetEventService>();

                collection.AddScoped<IConnectionManager, ConnectionManager>();
                collection.AddScoped<IMacroLauncher, MacroLauncher>();
                collection.AddScoped<IServiceManager, ServiceManager>();

                #endregion

                #region DataBase

                collection.AddScoped<IDataRepository<SettingsItem>, SettingsRepository>();
                collection.AddScoped<IDataRepository<Metadata>, MetadataRepository>();
                collection.AddScoped<DbRepository>();

                #endregion

                #region InstructionServices

                collection.AddService<AppService>(serviceManager);
                collection.AddService<OsService>(serviceManager);
                collection.AddService<KeyboardService>(serviceManager);
                collection.AddService<MouseService>(serviceManager);
                collection.AddService<TaskService>(serviceManager);

                #endregion

                #region NetworkConfiguration

                collection.AddRouting();
                collection.AddSignalR();

                #endregion

            });

            host.Build();
            host.ConfigureApplication();

            _ =  host.RunAppAsync();

            serviceManager.ServiceProvider = host.ServiceProvider;

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
}

static class ServiceCollectionExtend
{
    public static IServiceCollection AddService<T>(this IServiceCollection services, IServiceManager serviceManager) where T : class, IServiceBase
    {
        serviceManager.AddService<T>();

        services.AddSingleton<T>();

        return services;
    }
}