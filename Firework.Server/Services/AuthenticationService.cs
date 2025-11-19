using Firework.Core.Crypto;
using Firework.Dto.Devices;
using Firework.Models.Devices;
using Firework.Models.RequestContext;
using Firework.Server.Abstraction;

namespace Firework.Server.Services;

public class AuthenticationService : IAuthenticationService
{
    private static Dictionary<string, Device> ConnectedDevices { get; set; } = new();
    
    public string Authenticate(DeviceRegisterDto deviceRegisterDto, RequestContext requestContext)
    {
        var deviceHash = DeviceHashService.GenerateHash(deviceRegisterDto.Name, deviceRegisterDto.OperatingSystem,
            deviceRegisterDto.DeviceSignature);
        
        if (ConnectedDevices.TryGetValue(deviceHash, out var value))
            return value.Hash;
        
        var device = new Device
        {
            Hash = deviceHash,
            Name = deviceRegisterDto.Name,
            OperatingSystem = deviceRegisterDto.OperatingSystem,
            IP = requestContext.IP,
            LastConnected = DateTime.UtcNow,
            LastUpdate = DateTime.UtcNow,
            IsMaster = ConnectedDevices.Count == 0
        };
        
        ConnectedDevices.Add(device.Hash, device);
        
        return device.Hash;
    }

    public Device? GetDevice(string deviceHash)
    {
        if (ConnectedDevices.TryGetValue(deviceHash, out var device))
            return device;

        return null;
    }

    public void Logout(Device device)
    {
        
    }
}