using System.Collections.ObjectModel;
using Firework.Abstraction.Services.NetEventService;
using Firework.Models.Events;

namespace Firework.Core;

public class NetEventService : INetEventService
{
    private static List<NetworkEvent> NetworkEvents { get; } = new(100);
    private object locker = new();

    public event EventHandler<NetworkEvent>? OnEventAdded; 
    
    public void AddEvent(NetworkEvent netEvent)
    {
        lock (locker)
        { 
            NetworkEvents.Add(netEvent);
            OnEventAdded?.Invoke(this, netEvent);
        }
    }

    public List<NetworkEvent> GetEvents()
    {
        return NetworkEvents;
    }
}