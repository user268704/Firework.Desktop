using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace Firework.Core.Settings;

public class AutoRunHandler
{
    private readonly string _appName;
    private readonly string _exePath;
    private readonly string _startupShortcutPath;
    private readonly string _iconPath;


    public AutoRunHandler(string appName)
    {
        _appName = appName;
        _exePath = Assembly.GetExecutingAssembly().Location;
        string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        _startupShortcutPath = Path.Combine(startupFolderPath, $"{_appName}.lnk");
    }


    public void On()
    {
        if (IsInStartup())
        {
            return;
        }

        AddToRegistry();
        CreateShortcut();
    }

    public void Off()
    {
        if (!IsInStartup())
        {
            return;
        }

        RemoveFromRegistry();
        DeleteShortcut();
    }


    private bool IsInStartup()
    {
        return IsInRegistry() || File.Exists(_startupShortcutPath);
    }

    private void AddToRegistry()
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key?.SetValue(_appName, _exePath);
            }
        }
        catch (Exception ex)
        {
        }
    }

    private void RemoveFromRegistry()
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                key?.DeleteValue(_appName, false);
            }
        }
        catch (Exception ex)
        {

        }
    }

    private bool IsInRegistry()
    {
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
        {
            return key?.GetValue(_appName) != null;
        }
    }

    private void CreateShortcut()
    {
        try
        {
            string workingDirectory = Path.GetDirectoryName(_exePath);

            // Формируем скрипт PowerShell для создания ярлыка
            string script = $@"
            $WScriptShell = New-Object -ComObject WScript.Shell;
            $Shortcut = $WScriptShell.CreateShortcut(""{_startupShortcutPath}"");
            $Shortcut.TargetPath = ""{_exePath}"";
            $Shortcut.WorkingDirectory = ""{workingDirectory}"";
            $Shortcut.WindowStyle = 1;
            $Shortcut.Description = 'Firework';
            {(!string.IsNullOrEmpty(_iconPath) ? $"$Shortcut.IconLocation = \"{_iconPath}\";" : "")}
            $Shortcut.Save();";

            // Запускаем PowerShell и передаем скрипт
            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-NoProfile -Command \"{script}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            })?.WaitForExit();
        }
        catch (Exception ex)
        {
        }
    }

    private void DeleteShortcut()
    {
        try
        {
            if (File.Exists(_startupShortcutPath))
            {
                File.Delete(_startupShortcutPath);
            }
        }
        catch (Exception ex)
        {
        }
    }
}