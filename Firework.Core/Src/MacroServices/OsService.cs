#pragma warning disable
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Firework.Abstraction.Services;
using Firework.Core.Logs;
using Firework.Core.MacroServices.Attrubutes;
using Firework.Models.Events;
using NLog;

namespace Firework.Core.MacroServices;

public class OsService : ServiceBase
{
    public OsService(IServiceManager serviceManager) : base(serviceManager)
    {
    }
    
    [ActionService]
    public void SetClipboard(string value)
    {
        throw new NotImplementedException();
    }

    [ActionService]
    public string GetClipboard()
    {
        throw new NotImplementedException();
    }

    [ActionService(Alias = "shutdown")]
    public void PowerOff()
    {
        Process.Start("cmd", "shutdown /s /t 0");
    }

    [ActionService(Alias = "externalipv4")]
    public string GetExternalIp()
    {
        return GetExternalIPAddress();
    }

    [ActionService(Alias = "localipv4")]
    public string GetLocalIp()
    {
        return GetLocalIPAddress();
    }


    [ActionService]
    public void Reboot()
    {
    }

    [ActionService(Alias = "username")]
    public string GetUserName()
    {
        return Environment.UserDomainName + "\\" + Environment.UserName;
    }
    
    [ActionService]
    public string GetInfo()
    {
        // TODO: оптимизировать
        var result = new StringBuilder();

        PerformanceCounter cpuCounter = new("Processor", "% Processor Time", "_Total");
        var firstValue = cpuCounter.NextValue();
        Thread.Sleep(1000);

        var secondValue = cpuCounter.NextValue();

        var cult = new CultureInfo(CultureInfo.CurrentCulture.Name);

        new NetEventService().AddEvent(new NetworkEvent
        {
            Message = $"{cult.Name} {cult.NumberFormat.CurrencyDecimalSeparator}",
            EventType = NetworkEvent.TypeEvent.Information,
            Date = DateTime.Now,
        });

        result.Append("(cpu:" + secondValue.ToString(new CultureInfo(CultureInfo.CurrentCulture.Name)) + ";");
        result.Append("gpu:" + GetGpu().ToString(new CultureInfo(CultureInfo.CurrentCulture.Name)) + ";");
        result.Append("ram:" + GetRam().ToString(new CultureInfo(CultureInfo.CurrentCulture.Name)) + ")");

        return result.ToString();
    }

    private float GetGpu()
    {
        try
        {
            var category = new PerformanceCounterCategory("GPU Engine");
            string[] counterNames = category.GetInstanceNames();
            var gpuCounters = new List<PerformanceCounter>();
            var result = 0f;

            foreach (var counterName in counterNames)
                if (counterName.EndsWith("engtype_3D"))
                    foreach (var counter in category.GetCounters(counterName))
                        if (counter.CounterName == "Utilization Percentage")
                            gpuCounters.Add(counter);

            gpuCounters.ForEach(x => _ = x.NextValue());

            Thread.Sleep(1000);

            gpuCounters.ForEach(x => result += x.NextValue());

            return result;
        }
        catch
        {
            return 0f;
        }
    }

    private float GetRam()
    {
        var ramMonitor = //запрос к WMI для получения памяти ПК
            new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");

        foreach (ManagementObject objram in ramMonitor.Get())
        {
            var totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]); //общая память ОЗУ
            var busyRam = totalRam - Convert.ToUInt64(objram["FreePhysicalMemory"]); //занятная память = (total-free)

            return busyRam * 100 / totalRam; //вычисляем проценты занятой памяти
        }

        return 0;
    }

    string GetLocalIPAddress()
    {
        string localIp = "Not found";

        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            localIp = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString()
                      ?? "Not found";
        }
        catch (Exception ex)
        {
            localIp = $"Error: {ex.Message}";
        }

        return localIp;
    }

    string GetExternalIPAddress()
    {
        string externalIp = "Not found";
        try
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect("8.8.8.8", 80);
                if (socket.LocalEndPoint is IPEndPoint endPoint)
                {
                    externalIp = endPoint.Address.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            externalIp = $"Error {ex.Message}";
        }
        return externalIp;
    }
}