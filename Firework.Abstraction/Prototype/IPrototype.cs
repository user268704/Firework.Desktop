namespace Firework.Abstraction.Prototype;

public interface IPrototype<out T>
{
    public T Clone();
}