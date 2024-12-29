using Firework.Models.Events;

namespace Firework.Abstraction.Services.NetEventService;

public interface INetEventService
{
    void AddEvent(NetworkEvent networkEvent);
    List<NetworkEvent> GetEvents();
    event EventHandler<NetworkEvent>? OnEventAdded;
}