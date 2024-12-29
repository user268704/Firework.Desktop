using System.Diagnostics;
using Firework.Abstraction.Data;
using Firework.Core.Settings;
using Firework.Models.Settings;

namespace Firework.Core.Startup;

public class Startup
{
    private readonly IDataRepository<SettingsItem> _settingsRepository;

    public Startup(IDataRepository<SettingsItem> settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public void Start()
    {
        string appName = _settingsRepository.FindBy(x => x.UniqueKey == SettingsDefault.Names.ApplicationName).Value;

        AddApplicationToFirewallExceptions(appName, Process.GetCurrentProcess().MainModule.FileName);
    }

    public static void AddApplicationToFirewallExceptions(string appName, string appPath)
    {
        if (!IsApplicationInFirewallExceptions(appName))
        {
            string command = $"advfirewall firewall add rule name=\"{appName}\" dir=in action=allow program=\"{appPath}\" enable=yes";
            ExecuteCommand(command);
        }
    }

    private static bool IsApplicationInFirewallExceptions(string appName)
    {
        string command = $"advfirewall firewall show rule name=\"{appName}\"";
        string output = ExecuteCommand(command);
        return output.Contains(appName);
    }

    private static string ExecuteCommand(string command)
    {
        Process process = new Process();
        process.StartInfo.FileName = "netsh";
        process.StartInfo.Arguments = command;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }


}