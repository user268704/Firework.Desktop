using System.Text.Json;
using Firework.Models.Instructions;

namespace Firework.Models.Events;

public class NetworkEvent
{
    public enum TypeEvent
    {
        Success,
        Error,
        NewAction,
        Information,
        Connect,
        Disconnect
    }

    public TypeEvent EventType { get; set; }
    public DateTime Date { get; set; }
    public string Message { get; set; }
    public string ClientIp { get; set; }

    public List<InstructionInfo> Instructions { get; set; }


    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}