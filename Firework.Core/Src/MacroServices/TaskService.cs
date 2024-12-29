using System.ComponentModel;
using System.Diagnostics;
using Firework.Abstraction.Services;
using Firework.Core.MacroServices.Attrubutes;

namespace Firework.Core.MacroServices;

public class TaskService : ServiceBase
{
    public TaskService(IServiceManager serviceManager) : base(serviceManager)
    {
    }
    
    [ActionService(Alias = "run")]
    public string RunTask(string path, string arguments = "")
    {
        try
        {
            var process = Process.Start(path, arguments);

            return "success";
        }
        catch (Win32Exception e)
        {
            return e.Message;
        }
        catch (FileNotFoundException e)
        {
            return e.Message;
        }
    }

    [ActionService(Alias = "kill")]
    public void KillTask(string processName)
    {
        var processes = Process.GetProcesses();
        var findProcesses = processes.Where(process => process.ProcessName == processName);

        foreach (var process in findProcesses) process.Kill();
    }

    [ActionService(Alias = "close")]
    public void CloseTask(string processName)
    {
        var processes = Process.GetProcesses();
        var findProcesses = processes.Where(process => process.ProcessName == processName);

        foreach (var process in findProcesses) process.CloseMainWindow();
    }

    [ActionService(Alias = "killall")]
    public void KillAllTasks()
    {
    }

    [ActionService]
    public string GetInfo(string processName)
    {
        return default;
    }
}