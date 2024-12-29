using Firework.Abstraction.Services.NetEventService;
using NLog;

namespace Firework.Core.Logs;

public class NLogger
{
    private static readonly Logger LogNetEvent = LogManager.GetLogger("NetEvent");
    private static readonly Logger LogError = LogManager.GetLogger("Error");
    private static readonly Logger LogUserAction = LogManager.GetLogger("UserAction");
    private static readonly Logger LogDebug = LogManager.GetLogger("Debug");
    private static readonly Logger LogInfo = LogManager.GetLogger("Info");

    public static void NetEvent(string action, Type? caller = null)
    {
        LogNetEvent.Info(action);
        
        /*
        NetEventManager.AddEvent(new NetworkEvent
        {
            Date = DateTime.Now,
            Message = action,
            
        }); 
    */
    }

    public static void Error(string message, Exception exception = null)
    {
        LogError.Error(message, exception);
    }

    public static void UserAction(string actionMessage)
    {
        LogUserAction.Info(actionMessage);
    }

    public static void Debug(string message)
    {
        LogDebug.Debug(message);
    }

    public static void Trace(string trace, string from = "")
    {
        LogUserAction.Trace(trace);
    }

    public static void Info(string info)
    {
        LogInfo
            .WithProperty("pathToInfo", "D:\\Firewotk.Txt\\info.log")
            .Info(info);
    }

    public static void Info(string info, string path)
    {
        LogInfo
            .WithProperty("pathToInfo", path)
            .Info(info);
    }
    
    
}