using Firework.Dto.Instructions;

namespace Firework.Server.Abstraction;

public interface IMessagePackService
{
    public T Parse<T>(byte[] data);
    public byte[] ToMessagePack<T>(T data);
}