using Firework.Abstraction.Connection;
using Firework.Models.Server;
using Microsoft.AspNetCore.Mvc;

namespace Firework.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IDeviceAuthorizationService _deviceAuthorizationService;
    private readonly IDeviceConnectionService _deviceConnectionService;

    public AuthorizationController(IDeviceAuthorizationService deviceAuthorizationService,
        IDeviceConnectionService deviceConnectionService)
    {
        _deviceAuthorizationService = deviceAuthorizationService;
        _deviceConnectionService = deviceConnectionService;
    }

    /// <summary>
    /// Проверяет авторизацию устройства
    /// </summary>
    [HttpGet("check/{connectionId}")]
    public ActionResult<bool> CheckAuthorization(string connectionId)
    {
        var isAuthorized = _deviceAuthorizationService.IsDeviceAuthorized(connectionId);
        return Ok(isAuthorized);
    }

    /// <summary>
    /// Получает информацию об авторизованном устройстве
    /// </summary>
    [HttpGet("device/{connectionId}")]
    public ActionResult<DeviceConnectionInfo> GetAuthorizedDevice(string connectionId)
    {
        var device = _deviceAuthorizationService.GetAuthorizedDevice(connectionId);
        if (device == null)
            return NotFound($"Устройство с ConnectionId {connectionId} не авторизовано");

        return Ok(device);
    }

    /// <summary>
    /// Отзывает авторизацию устройства
    /// </summary>
    [HttpDelete("revoke/{connectionId}")]
    public ActionResult RevokeAuthorization(string connectionId)
    {
        var revoked = _deviceAuthorizationService.RevokeAuthorization(connectionId);
        if (!revoked)
            return NotFound($"Устройство с ConnectionId {connectionId} не найдено или не авторизовано");

        return Ok();
    }

    /// <summary>
    /// Проверяет доступ к сервису
    /// </summary>
    [HttpGet("service-access/{connectionId}/{serviceName}")]
    public ActionResult<bool> CheckServiceAccess(string connectionId, string serviceName)
    {
        var hasAccess = _deviceAuthorizationService.HasServiceAccess(connectionId, serviceName);
        return Ok(hasAccess);
    }

    /// <summary>
    /// Проверяет доступ к действию
    /// </summary>
    [HttpGet("action-access/{connectionId}/{serviceName}/{actionName}")]
    public ActionResult<bool> CheckActionAccess(string connectionId, string serviceName, string actionName)
    {
        var hasAccess = _deviceAuthorizationService.HasActionAccess(connectionId, serviceName, actionName);
        return Ok(hasAccess);
    }

    /// <summary>
    /// Получает все авторизованные устройства
    /// </summary>
    [HttpGet("authorized-devices")]
    public ActionResult<IEnumerable<DeviceConnectionInfo>> GetAuthorizedDevices()
    {
        var allDevices = _deviceConnectionService.GetAllDevices();
        var authorizedDevices = allDevices.Where(d => d.State == ConnectionState.Connected);
        return Ok(authorizedDevices);
    }

    /// <summary>
    /// Получает количество авторизованных устройств
    /// </summary>
    [HttpGet("authorized-count")]
    public ActionResult<int> GetAuthorizedDevicesCount()
    {
        var allDevices = _deviceConnectionService.GetAllDevices();
        var authorizedCount = allDevices.Count(d => d.State == ConnectionState.Connected);
        return Ok(authorizedCount);
    }
}

