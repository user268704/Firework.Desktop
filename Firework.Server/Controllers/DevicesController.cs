using Firework.Abstraction.Connection;
using Firework.Models.Server;
using Microsoft.AspNetCore.Mvc;

namespace Firework.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceConnectionService _deviceConnectionService;

    public DevicesController(IDeviceConnectionService deviceConnectionService)
    {
        _deviceConnectionService = deviceConnectionService;
    }

    /// <summary>
    /// Получает список всех подключенных устройств
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<DeviceConnectionInfo>> GetDevices()
    {
        var devices = _deviceConnectionService.GetAllDevices();
        return Ok(devices);
    }

    /// <summary>
    /// Получает информацию об устройстве по ID соединения
    /// </summary>
    [HttpGet("connection/{connectionId}")]
    public ActionResult<DeviceConnectionInfo> GetDeviceByConnectionId(string connectionId)
    {
        var device = _deviceConnectionService.GetDevice(connectionId);
        if (device == null)
            return NotFound($"Устройство с ConnectionId {connectionId} не найдено");

        return Ok(device);
    }

    /// <summary>
    /// Получает информацию об устройстве по имени
    /// </summary>
    [HttpGet("name/{deviceName}")]
    public ActionResult<DeviceConnectionInfo> GetDeviceByName(string deviceName)
    {
        var device = _deviceConnectionService.GetDeviceByName(deviceName);
        if (device == null)
            return NotFound($"Устройство с именем {deviceName} не найдено");

        return Ok(device);
    }

    /// <summary>
    /// Получает количество подключенных устройств
    /// </summary>
    [HttpGet("count")]
    public ActionResult<int> GetDevicesCount()
    {
        var count = _deviceConnectionService.GetConnectedDevicesCount();
        return Ok(count);
    }

    /// <summary>
    /// Проверяет, подключено ли устройство
    /// </summary>
    [HttpGet("connected/{connectionId}")]
    public ActionResult<bool> IsDeviceConnected(string connectionId)
    {
        var isConnected = _deviceConnectionService.IsDeviceConnected(connectionId);
        return Ok(isConnected);
    }

    /// <summary>
    /// Получает ID соединения для устройства по имени
    /// </summary>
    [HttpGet("connection-id/{deviceName}")]
    public ActionResult<string> GetConnectionId(string deviceName)
    {
        var connectionId = _deviceConnectionService.GetConnectionId(deviceName);
        if (connectionId == null)
            return NotFound($"Устройство с именем {deviceName} не найдено");

        return Ok(connectionId);
    }

    /// <summary>
    /// Обновляет состояние устройства
    /// </summary>
    [HttpPut("state/{connectionId}")]
    public ActionResult UpdateDeviceState(string connectionId, [FromBody] ConnectionState state)
    {
        var updated = _deviceConnectionService.UpdateDeviceState(connectionId, state);
        if (!updated)
            return NotFound($"Устройство с ConnectionId {connectionId} не найдено");

        return Ok();
    }

    /// <summary>
    /// Удаляет устройство из списка подключенных
    /// </summary>
    [HttpDelete("{connectionId}")]
    public ActionResult RemoveDevice(string connectionId)
    {
        var removed = _deviceConnectionService.RemoveDevice(connectionId);
        if (!removed)
            return NotFound($"Устройство с ConnectionId {connectionId} не найдено");

        return Ok();
    }
}
