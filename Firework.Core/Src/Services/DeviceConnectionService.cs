using Firework.Abstraction.Connection;
using Firework.Models.Server;
using Microsoft.Extensions.Logging;

namespace Firework.Core.Services;

public class DeviceConnectionService : IDeviceConnectionService
{
    private readonly Dictionary<string, DeviceConnectionInfo> _connectedDevices = new();
    private readonly Dictionary<string, DeviceConnectionInfo> _devicesByName = new();
    private readonly object _lockObject = new();
    private readonly ILogger<DeviceConnectionService> _logger;

    public DeviceConnectionService(ILogger<DeviceConnectionService> logger)
    {
        _logger = logger;
    }

    public DeviceConnectionInfo AddDevice(string connectionId, object deviceInfo)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            throw new ArgumentException("ConnectionId не может быть пустым", nameof(connectionId));
        
        if (deviceInfo == null)
            throw new ArgumentNullException(nameof(deviceInfo));
        
        // Приводим к нужному типу
        if (deviceInfo is not Firework.Dto.Results.DeviceInfoHandshake handshake)
            throw new ArgumentException("Неверный тип информации об устройстве", nameof(deviceInfo));
        
        if (string.IsNullOrWhiteSpace(handshake.DeviceName))
            throw new ArgumentException("Имя устройства не может быть пустым", nameof(deviceInfo));
        
        if (string.IsNullOrWhiteSpace(handshake.Ip))
            throw new ArgumentException("IP адрес не может быть пустым", nameof(deviceInfo));

        lock (_lockObject)
        {
            // Проверяем, не подключено ли уже устройство с таким именем
            if (_devicesByName.ContainsKey(handshake.DeviceName))
            {
                var existingDevice = _devicesByName[handshake.DeviceName];
                _logger.LogWarning("Устройство с именем {DeviceName} уже подключено с ConnectionId {ExistingConnectionId}", 
                    handshake.DeviceName, existingDevice.ConnectionId);
                
                // Удаляем старое подключение
                RemoveDevice(existingDevice.ConnectionId);
            }

            var deviceConnectionInfo = new DeviceConnectionInfo
            {
                ConnectionId = connectionId,
                DeviceName = handshake.DeviceName,
                IpAddress = handshake.Ip,
                State = ConnectionState.Connected,
                ConnectedAt = DateTime.Now,
                LastActivity = DateTime.Now,
                ClientVersion = handshake.ClientVersion,
                OperatingSystem = handshake.OperatingSystem ?? handshake.Os,
                Architecture = handshake.Architecture
            };

            _connectedDevices[connectionId] = deviceConnectionInfo;
            _devicesByName[handshake.DeviceName] = deviceConnectionInfo;

            _logger.LogInformation("Добавлено новое устройство: {DeviceName} ({Ip}) с ConnectionId {ConnectionId}", 
                handshake.DeviceName, handshake.Ip, connectionId);

            OnDeviceAdded?.Invoke(this, deviceConnectionInfo);
            
            return deviceConnectionInfo;
        }
    }

    public bool RemoveDevice(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return false;

        lock (_lockObject)
        {
            if (!_connectedDevices.TryGetValue(connectionId, out var deviceInfo))
            {
                _logger.LogDebug("Попытка удалить несуществующее устройство с ConnectionId {ConnectionId}", connectionId);
                return false;
            }

            _connectedDevices.Remove(connectionId);
            _devicesByName.Remove(deviceInfo.DeviceName);

            _logger.LogInformation("Удалено устройство: {DeviceName} ({Ip}) с ConnectionId {ConnectionId}", 
                deviceInfo.DeviceName, deviceInfo.IpAddress, connectionId);

            OnDeviceRemoved?.Invoke(this, deviceInfo);
            
            return true;
        }
    }

    public DeviceConnectionInfo? GetDevice(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return null;

        lock (_lockObject)
        {
            return _connectedDevices.TryGetValue(connectionId, out var deviceInfo) ? deviceInfo : null;
        }
    }

    public DeviceConnectionInfo? GetDeviceByName(string deviceName)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
            return null;

        lock (_lockObject)
        {
            return _devicesByName.TryGetValue(deviceName, out var deviceInfo) ? deviceInfo : null;
        }
    }

    public IEnumerable<DeviceConnectionInfo> GetAllDevices()
    {
        lock (_lockObject)
        {
            return _connectedDevices.Values.ToList();
        }
    }

    public bool UpdateDeviceState(string connectionId, ConnectionState state)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return false;

        lock (_lockObject)
        {
            if (!_connectedDevices.TryGetValue(connectionId, out var deviceInfo))
            {
                _logger.LogDebug("Попытка обновить состояние несуществующего устройства с ConnectionId {ConnectionId}", connectionId);
                return false;
            }

            var oldState = deviceInfo.State;
            deviceInfo.State = state;
            deviceInfo.LastActivity = DateTime.Now;

            _logger.LogDebug("Обновлено состояние устройства {DeviceName}: {OldState} -> {NewState}", 
                deviceInfo.DeviceName, oldState, state);

            OnDeviceStateChanged?.Invoke(this, deviceInfo);
            
            return true;
        }
    }

    public bool UpdateDevice(string connectionId, Action<DeviceConnectionInfo> updateAction)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return false;
        
        if (updateAction == null)
            throw new ArgumentNullException(nameof(updateAction));

        lock (_lockObject)
        {
            if (!_connectedDevices.TryGetValue(connectionId, out var deviceInfo))
            {
                _logger.LogDebug("Попытка обновить несуществующее устройство с ConnectionId {ConnectionId}", connectionId);
                return false;
            }

            updateAction(deviceInfo);
            deviceInfo.LastActivity = DateTime.Now;

            _logger.LogDebug("Обновлено устройство {DeviceName} с ConnectionId {ConnectionId}", 
                deviceInfo.DeviceName, connectionId);

            OnDeviceStateChanged?.Invoke(this, deviceInfo);
            
            return true;
        }
    }

    public int GetConnectedDevicesCount()
    {
        lock (_lockObject)
        {
            return _connectedDevices.Count;
        }
    }

    public bool IsDeviceConnected(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return false;

        lock (_lockObject)
        {
            return _connectedDevices.ContainsKey(connectionId);
        }
    }

    public string? GetConnectionId(string deviceName)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
            return null;

        lock (_lockObject)
        {
            return _devicesByName.TryGetValue(deviceName, out var deviceInfo) ? deviceInfo.ConnectionId : null;
        }
    }

    public event EventHandler<DeviceConnectionInfo>? OnDeviceStateChanged;
    public event EventHandler<DeviceConnectionInfo>? OnDeviceAdded;
    public event EventHandler<DeviceConnectionInfo>? OnDeviceRemoved;
}
