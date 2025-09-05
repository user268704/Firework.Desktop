using Microsoft.Extensions.Logging;

namespace Firework.Core.Services.PipeBroker;

public class PipeServiceBuilder
{
    private readonly PipeService _serviceResult;
    
    
    public PipeServiceBuilder(string pipeName)
    {
        _serviceResult = new PipeService();

        _serviceResult.PipeName = pipeName;
    }
    
    public PipeServiceBuilder WithLogging(ILogger logger)
    {
        _serviceResult.IsEnableLogging = true;
        _serviceResult.Logger = logger;
        
        return this;
    }
    
    public PipeServiceBuilder WithMessageHandler(Action<byte[]> callback)
    {
        _serviceResult.OnReceiveMessage += callback;
        
        return this;
    }
    
    public PipeServiceBuilder OnConnected(Action callback)
    {
        _serviceResult.OnConnected += callback;
        
        return this;
    }

    public PipeServiceBuilder AsClient()
    {
        _serviceResult.IsClient = true;
        
        return this;
    }
    
    public PipeService Build()
    {
        return _serviceResult;
    }
}