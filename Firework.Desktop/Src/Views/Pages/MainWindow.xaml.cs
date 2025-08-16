using System.Windows;
using Firework.Desktop.ViewModel;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Controls;
namespace Firework.Desktop.Views.Pages;


public partial class MainWindow : FluentWindow
{
    public MainWindowViewModel ViewModel { get; set; }
    
    public MainWindow(MainWindowViewModel mainWindowViewModel,
        INavigationViewPageProvider pageService)
    {
        InitializeComponent();
        
        RootNavigation.SetPageProviderService(pageService);
        ViewModel = mainWindowViewModel;

        DataContext = ViewModel;

        Loaded += (_, _) => RootNavigation.Navigate(typeof(MainPage));
    }

    private void WindowStateChanged(object? sender, EventArgs e)
    {
        return;

        //var isMinimizeInTray = _settingsRepository.GetAll().First(x => x.UniqueKey == SettingsDefault.Names.MinimizeInTray);

        /*
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
    */
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