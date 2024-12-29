using Microsoft.Extensions.Configuration;

namespace Firework.Abstraction.Configuration;

public interface IConfigurationExtend : IConfiguration
{
    public void SetValue<T>(string key, T value);
}