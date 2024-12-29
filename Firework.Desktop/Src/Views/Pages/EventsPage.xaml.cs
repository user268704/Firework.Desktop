using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Firework.Abstraction.Services.NetEventService;
using Firework.Core;
using Firework.Models.Events;

namespace Firework.Desktop.Views.Pages;

public partial class EventsPage : Page
{
    private readonly INetEventService _netEventService;
    public ObservableCollection<NetworkEvent> NetworkEventArgsList { get; set; }
    
    public EventsPage(INetEventService netEventService)
    {
        _netEventService = netEventService;
        InitializeComponent();
        DataContext = this;
        
        NetworkEventArgsList = new ObservableCollection<NetworkEvent>();
        
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _netEventService.OnEventAdded -= OnNetEventAdded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SetNetEvents(_netEventService.GetEvents().ToList());

        _netEventService.OnEventAdded += OnNetEventAdded;
    }

    private void OnNetEventAdded(object? o, NetworkEvent @event)
    {
        Dispatcher.Invoke(() => NetworkEventArgsList.Add(@event));
    }

    private void SetNetEvents(List<NetworkEvent> netEvents)
    {
        NetworkEventArgsList.Clear();

        foreach (var netEvent in netEvents) NetworkEventArgsList.Add(netEvent);
    }
}