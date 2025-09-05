using Firework.Models.Server;

namespace Firework.Abstraction.Connection;

public interface IDeviceConnectionService
{
    /// <summary>
    /// Добавляет новое устройство в список подключенных
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <param name="deviceInfo">Информация об устройстве</param>
    /// <returns>Информация о подключенном устройстве</returns>
    DeviceConnectionInfo AddDevice(string connectionId, object deviceInfo);
    
    /// <summary>
    /// Удаляет устройство из списка подключенных
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <returns>True если устройство было найдено и удалено</returns>
    bool RemoveDevice(string connectionId);
    
    /// <summary>
    /// Получает информацию об устройстве по ID соединения
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <returns>Информация об устройстве или null если не найдено</returns>
    DeviceConnectionInfo? GetDevice(string connectionId);
    
    /// <summary>
    /// Получает информацию об устройстве по имени устройства
    /// </summary>
    /// <param name="deviceName">Имя устройства</param>
    /// <returns>Информация об устройстве или null если не найдено</returns>
    DeviceConnectionInfo? GetDeviceByName(string deviceName);
    
    /// <summary>
    /// Получает все подключенные устройства
    /// </summary>
    /// <returns>Список всех подключенных устройств</returns>
    IEnumerable<DeviceConnectionInfo> GetAllDevices();
    
    /// <summary>
    /// Обновляет состояние устройства
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <param name="state">Новое состояние</param>
    /// <returns>True если устройство было найдено и обновлено</returns>
    bool UpdateDeviceState(string connectionId, ConnectionState state);
    
    /// <summary>
    /// Обновляет свойства устройства
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <param name="updateAction">Действие для обновления свойств</param>
    /// <returns>True если устройство было найдено и обновлено</returns>
    bool UpdateDevice(string connectionId, Action<DeviceConnectionInfo> updateAction);
    
    /// <summary>
    /// Получает количество подключенных устройств
    /// </summary>
    /// <returns>Количество подключенных устройств</returns>
    int GetConnectedDevicesCount();
    
    /// <summary>
    /// Проверяет, подключено ли устройство
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <returns>True если устройство подключено</returns>
    bool IsDeviceConnected(string connectionId);
    
    /// <summary>
    /// Получает ID соединения SignalR для устройства по имени
    /// </summary>
    /// <param name="deviceName">Имя устройства</param>
    /// <returns>ID соединения или null если устройство не найдено</returns>
    string? GetConnectionId(string deviceName);
    
    /// <summary>
    /// Событие, возникающее при изменении состояния устройства
    /// </summary>
    event EventHandler<DeviceConnectionInfo> OnDeviceStateChanged;
    
    /// <summary>
    /// Событие, возникающее при добавлении нового устройства
    /// </summary>
    event EventHandler<DeviceConnectionInfo> OnDeviceAdded;
    
    /// <summary>
    /// Событие, возникающее при удалении устройства
    /// </summary>
    event EventHandler<DeviceConnectionInfo> OnDeviceRemoved;
}
