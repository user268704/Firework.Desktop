using FluentResults;
using MessagePack;

namespace Firework.Server.Abstraction;

public interface IMessagePackService
{
    MessagePackSerializerOptions Options { get; }
    Result<T> Deserialize<T>(ReadOnlyMemory<byte> payload);
    Result<object?> Deserialize(ReadOnlyMemory<byte> payload, Type targetType);
    Result<byte[]> Serialize<T>(T value);
}