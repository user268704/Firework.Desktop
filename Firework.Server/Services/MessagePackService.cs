using Firework.Server.Abstraction;
using FluentResults;
using MessagePack;

namespace Firework.Server.Services;

public sealed class MessagePackService : IMessagePackService
{
    private static readonly MessagePackSerializerOptions SerializerOptions =
        MessagePackSerializerOptions.Standard
            .WithResolver(MessagePack.Resolvers.ContractlessStandardResolver.Instance)
            .WithCompression(MessagePackCompression.Lz4BlockArray);

    public static MessagePackSerializerOptions SharedOptions => SerializerOptions;

    public MessagePackSerializerOptions Options => SerializerOptions;

    public Result<T> Deserialize<T>(ReadOnlyMemory<byte> payload)
    {
        try
        {
            var result = MessagePackSerializer.Deserialize<T>(payload, SerializerOptions);
            return Result.Ok(result);
        }
        catch (Exception exception)
        {
            return Result.Fail(new Error("Failed to deserialize payload.").CausedBy(exception));
        }
    }

    public Result<object?> Deserialize(ReadOnlyMemory<byte> payload, Type targetType)
    {
        try
        {
            var value = MessagePackSerializer.Deserialize(targetType, payload, SerializerOptions);
            return Result.Ok(value);
        }
        catch (Exception exception)
        {
            return Result.Fail(new Error("Failed to deserialize payload.").CausedBy(exception));
        }
    }

    public Result<byte[]> Serialize<T>(T value)
    {
        try
        {
            var bytes = MessagePackSerializer.Serialize(value, SerializerOptions);
            return Result.Ok(bytes);
        }
        catch (Exception exception)
        {
            return Result.Fail(new Error("Failed to serialize payload.").CausedBy(exception));
        }
    }
}

