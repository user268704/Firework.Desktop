using System.Windows;
using System.Windows.Controls;
using Firework.Abstraction.HttpServer;
using Firework.Models.Server;

namespace Firework.Desktop.Views.Components;

public partial class ConnectionPanel : UserControl
{
    private readonly IConnectionManager _connectionManager;

    public static readonly DependencyProperty ClientIpProperty =
        DependencyProperty.Register("ClientIp", typeof(string), typeof(ConnectionPanel), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty ClientNameProperty =
        DependencyProperty.Register("ClientName", typeof(string), typeof(ConnectionPanel), new PropertyMetadata(default(string)));

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register("State", typeof(ConnectionState), typeof(ConnectionPanel), new PropertyMetadata(default(ConnectionState)));

    public string ClientIp
    {
        get => (string)GetValue(ClientIpProperty);
        set => SetValue(ClientIpProperty, value);
    }

    public string ClientName
    {
        get => (string)GetValue(ClientNameProperty);
        set => SetValue(ClientNameProperty, value);
    }

    public ConnectionState State
    {
        get => (ConnectionState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }


    public ConnectionPanel()
    {
        InitializeComponent();
        DataContext = this;
    }
}