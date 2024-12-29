using System;
using Wpf.Ui.Controls;
using System.ComponentModel;
using Firework.Models.Server;
using Firework.Desktop.Views.Pages;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firework.Abstraction.Connection;

namespace Firework.Desktop.ViewModel;

public sealed partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly IConnectionManager _connectionManager;
    private ObservableCollection<NavigationViewItem> _menuItems;
    private ObservableCollection<NavigationViewItem> _footerMenuItems;
    private ConnectionInfo _connectionInfo;

    public ObservableCollection<NavigationViewItem> MenuItems
    {
        get => _menuItems;
        set
        {
            if (Equals(value, _menuItems)) return;
            _menuItems = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<NavigationViewItem> FooterMenuItems
    {
        get => _footerMenuItems;
        set
        {
            if (Equals(value, _footerMenuItems)) return;
            _footerMenuItems = value;
            OnPropertyChanged();
        }
    }

    public ConnectionInfo ConnectionInfo
    {
        get => _connectionInfo;
        set
        {
            if (Equals(value, _connectionInfo)) return;
            _connectionInfo = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel(IConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
        ConnectionInfo = _connectionManager.GetCurrentConnectionInfo();
        _connectionManager.OnConnectionChanged += OnConnectionChanged;
        
        MenuItems = new ObservableCollection<NavigationViewItem>
        {
            new("Главная", SymbolRegular.Home24, typeof(MainPage)),
            new("Логи", SymbolRegular.Settings24, typeof(EventsPage)),
            new("Подключения", SymbolRegular.NetworkAdapter16, typeof(NetworkManagerPage)),
        };

        FooterMenuItems = new ObservableCollection<NavigationViewItem>
        {
            new("Настройки", SymbolRegular.Settings24, typeof(SettingsPage)),
        };
    }

    [RelayCommand]
    public void ShowDialogConnectionInfo()
    {

    }

    private void OnConnectionChanged(object? sender, ConnectionInfo info)
    {
        ConnectionInfo = info;
    }

    public void Dispose()
    {
        _connectionManager.OnConnectionChanged -= OnConnectionChanged;
    }
}