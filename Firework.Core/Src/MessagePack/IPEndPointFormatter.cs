using System.Net;
using MessagePack;
using MessagePack.Formatters;

namespace Firework.Core.MessagePack;

public class IPEndPointFormatter : IMessagePackFormatter<IPEndPoint>
{
    public void Serialize(ref MessagePackWriter writer, IPEndPoint value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(2);
        writer.Write(value.Address.ToString());
        writer.Write(value.Port);
    }

    public IPEndPoint Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        reader.ReadArrayHeader();
        var address = IPAddress.Parse(reader.ReadString());
        var port = reader.ReadInt32();
        return new IPEndPoint(address, port);
    }
}