using Firework.Models.Server;

namespace Firework.Abstraction.Connection;

public interface IDeviceAuthorizationService
{
    /// <summary>
    /// Проверяет, авторизовано ли устройство
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <returns>True если устройство авторизовано</returns>
    bool IsDeviceAuthorized(string connectionId);
    
    /// <summary>
    /// Получает информацию об авторизованном устройстве
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <returns>Информация об устройстве или null если не авторизовано</returns>
    DeviceConnectionInfo? GetAuthorizedDevice(string connectionId);
    
    /// <summary>
    /// Авторизует устройство
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <param name="deviceInfo">Информация об устройстве</param>
    /// <returns>Информация об авторизованном устройстве</returns>
    DeviceConnectionInfo AuthorizeDevice(string connectionId, object deviceInfo);
    
    /// <summary>
    /// Отзывает авторизацию устройства
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <returns>True если авторизация была отозвана</returns>
    bool RevokeAuthorization(string connectionId);
    
    /// <summary>
    /// Проверяет права доступа к сервису
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <param name="serviceName">Имя сервиса</param>
    /// <returns>True если есть доступ к сервису</returns>
    bool HasServiceAccess(string connectionId, string serviceName);
    
    /// <summary>
    /// Проверяет права доступа к действию
    /// </summary>
    /// <param name="connectionId">ID соединения SignalR</param>
    /// <param name="serviceName">Имя сервиса</param>
    /// <param name="actionName">Имя действия</param>
    /// <returns>True если есть доступ к действию</returns>
    bool HasActionAccess(string connectionId, string serviceName, string actionName);
    
    /// <summary>
    /// Событие, возникающее при авторизации устройства
    /// </summary>
    event EventHandler<DeviceConnectionInfo> OnDeviceAuthorized;
    
    /// <summary>
    /// Событие, возникающее при отзыве авторизации устройства
    /// </summary>
    event EventHandler<DeviceConnectionInfo> OnDeviceAuthorizationRevoked;
}

