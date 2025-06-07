using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Firework.Abstraction.Data;
using Firework.Core.Settings;
using Firework.Desktop.ViewModel;
using Firework.Desktop.Views.Components;
using Firework.Models.Data;
using Firework.Models.Server;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Controls;
namespace Firework.Desktop.Views.Pages;


public partial class MainWindow : FluentWindow
{
    private readonly IDataRepository<SettingsItem> _settingsRepository;

    public MainWindowViewModel ViewModel { get; set; }
    
    public MainWindow(MainWindowViewModel mainWindowViewModel,
        IDataRepository<SettingsItem> settingsRepository,
        INavigationViewPageProvider pageService)
    {
        InitializeComponent();
        _settingsRepository = settingsRepository;
        
        
        RootNavigation.SetPageProviderService(pageService);
        ViewModel = mainWindowViewModel;

        DataContext = ViewModel;

        Loaded += (_, _) => RootNavigation.Navigate(typeof(MainPage));
    }

    private void WindowStateChanged(object? sender, EventArgs e)
    {
        return;

        var isMinimizeInTray = _settingsRepository.GetAll().First(x => x.UniqueKey == SettingsDefault.Names.MinimizeInTray);

        if (bool.Parse(isMinimizeInTray.Value))
        {
            if (WindowState == WindowState.Minimized)
            {
                // скрытие окон
                foreach (Window window in OwnedWindows)
                {
                    window.WindowState = WindowState.Minimized;
                    window.ShowInTaskbar = false;
                }

                TrayIcon.Visibility = Visibility.Visible;
                ShowInTaskbar = false;
            }

            if (WindowState is WindowState.Normal or WindowState.Maximized)
            {
                // восстановление окон
                foreach (Window window in OwnedWindows)
                {
                    window.WindowState = WindowState.Normal;
                    window.ShowInTaskbar = true;
                }

                TrayIcon.Visibility = Visibility.Collapsed;
                ShowInTaskbar = true;
            }
        }
    }


    private void MenuItemOpenWindow(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Normal;
        ShowInTaskbar = true;

        foreach (Window window in OwnedWindows)
        {
            window.WindowState = WindowState.Normal;
            window.ShowInTaskbar = true;
        }
    }

    /*
    private void MenuItemCloseWindow(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
    */

    /*
    protected override void OnClosed(EventArgs e)
    {
        Application.Current.Shutdown();
    }
    */

    private async void UIElement_OnMouseLeftButtonUp(object sender, RoutedEventArgs routedEventArgs)
    {


        //ConnectFlyout.Show();
    }
}