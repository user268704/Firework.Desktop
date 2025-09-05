using Wpf.Ui.Controls;
using Firework.Models.Server;
using Firework.Desktop.Views.Pages;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firework.Abstraction.Connection;
using System.Net;

namespace Firework.Desktop.ViewModel;

public sealed partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly IConnectionManager _connectionManager;
    private ObservableCollection<NavigationViewItem> _menuItems;
    private ObservableCollection<NavigationViewItem> _footerMenuItems;
    private ConnectionInfo _connectionInfo;
    private string _serverAddress;

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

    public string ServerAddress
    {
        get => _serverAddress;
        set
        {
            if (Equals(value, _serverAddress)) return;
            _serverAddress = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel(IConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
        ConnectionInfo = _connectionManager.GetCurrentConnectionInfo();
        _connectionManager.OnConnectionChanged += OnConnectionChanged;
        
        ServerAddress = GetLocalIpAddress();
        
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

    [RelayCommand]
    public void SimulateClientConnection()
    {
        var testConnectionInfo = new ConnectionInfo
        {
            ClientName = "TestDevice",
            ClientIp = "192.168.1.100",
            IsConnected = true,
            State = ConnectionState.Connected,
            DateConnected = DateTime.Now
        };
        
        _connectionManager.SetConnectionInfo(testConnectionInfo);
    }

    [RelayCommand]
    public void SimulateClientDisconnection()
    {
        // Симуляция отключения клиента для тестирования
        var testConnectionInfo = new ConnectionInfo
        {
            ClientName = "ожидание подключения",
            ClientIp = "0.0.0.0",
            IsConnected = false,
            State = ConnectionState.NotConnected,
            DateConnected = DateTime.MinValue
        };
        
        _connectionManager.SetConnectionInfo(testConnectionInfo);
    }

    private void OnConnectionChanged(object? sender, ConnectionInfo info)
    {
        ConnectionInfo = info;
    }

    private string GetLocalIpAddress()
    {
        int port = 5000;
        
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return $"{ip}:{port}";
                }
            }
        }
        catch
        {
            // В случае ошибки возвращаем localhost
        }
        return $"localhost:{port}";
    }

    public void Dispose()
    {
        _connectionManager.OnConnectionChanged -= OnConnectionChanged;
    }
}