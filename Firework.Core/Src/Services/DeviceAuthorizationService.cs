using Firework.Abstraction.Connection;
using Firework.Models.Server;
using Microsoft.Extensions.Logging;

namespace Firework.Core.Services;

public class DeviceAuthorizationService : IDeviceAuthorizationService
{
    private readonly IDeviceConnectionService _deviceConnectionService;
    private readonly ILogger<DeviceAuthorizationService> _logger;
    
    private readonly HashSet<string> _allowedServices = new()
    {
        "os", "keyboard", "mouse", "app", "task"
    };
    
    private readonly HashSet<string> _forbiddenActions = new()
    {
        "shutdown", "restart", "format", "delete", "uninstall"
    };

    public DeviceAuthorizationService(IDeviceConnectionService deviceConnectionService, 
        ILogger<DeviceAuthorizationService> logger)
    {
        _deviceConnectionService = deviceConnectionService;
        _logger = logger;
    }

    public bool IsDeviceAuthorized(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return false;

        var device = _deviceConnectionService.GetDevice(connectionId);
        return device != null && device.State == ConnectionState.Connected;
    }

    public DeviceConnectionInfo? GetAuthorizedDevice(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return null;

        var device = _deviceConnectionService.GetDevice(connectionId);
        return device?.State == ConnectionState.Connected ? device : null;
    }

    public DeviceConnectionInfo AuthorizeDevice(string connectionId, object deviceInfo)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            throw new ArgumentException("ConnectionId не может быть пустым", nameof(connectionId));
        
        if (deviceInfo == null)
            throw new ArgumentNullException(nameof(deviceInfo));

        var device = _deviceConnectionService.AddDevice(connectionId, deviceInfo);
        
        _logger.LogInformation("Устройство авторизовано: {DeviceName} ({IpAddress})", 
            device.DeviceName, device.IpAddress);

        OnDeviceAuthorized?.Invoke(this, device);
        
        return device;
    }

    public bool RevokeAuthorization(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return false;

        var device = _deviceConnectionService.GetDevice(connectionId);
        if (device == null)
            return false;

        var removed = _deviceConnectionService.RemoveDevice(connectionId);
        if (removed)
        {
            _logger.LogInformation("Авторизация отозвана для устройства: {DeviceName}", device.DeviceName);
            OnDeviceAuthorizationRevoked?.Invoke(this, device);
        }
        
        return removed;
    }

    public bool HasServiceAccess(string connectionId, string serviceName)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(serviceName))
            return false;

        if (!IsDeviceAuthorized(connectionId))
        {
            _logger.LogWarning("Попытка доступа к сервису {ServiceName} неавторизованным устройством {ConnectionId}", 
                serviceName, connectionId);
            return false;
        }

        var hasAccess = _allowedServices.Contains(serviceName.ToLowerInvariant());
        
        if (!hasAccess)
        {
            var device = GetAuthorizedDevice(connectionId);
            _logger.LogWarning("Попытка доступа к запрещенному сервису {ServiceName} устройством {DeviceName}", 
                serviceName, device?.DeviceName ?? "Unknown");
        }

        return hasAccess;
    }

    public bool HasActionAccess(string connectionId, string serviceName, string actionName)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || 
            string.IsNullOrWhiteSpace(serviceName) || 
            string.IsNullOrWhiteSpace(actionName))
            return false;

        if (!HasServiceAccess(connectionId, serviceName))
            return false;

        var isForbidden = _forbiddenActions.Contains(actionName.ToLowerInvariant());
        
        if (isForbidden)
        {
            var device = GetAuthorizedDevice(connectionId);
            _logger.LogWarning("Попытка выполнения запрещенного действия {ActionName} в сервисе {ServiceName} устройством {DeviceName}", 
                actionName, serviceName, device?.DeviceName ?? "Unknown");
            return false;
        }

        return true;
    }

    public event EventHandler<DeviceConnectionInfo>? OnDeviceAuthorized;
    public event EventHandler<DeviceConnectionInfo>? OnDeviceAuthorizationRevoked;
}
