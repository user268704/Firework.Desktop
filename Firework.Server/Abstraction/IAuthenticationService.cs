using Firework.Dto.Devices;
using Firework.Models.Devices;
using Firework.Models.RequestContext;

namespace Firework.Server.Abstraction;

public interface IAuthenticationService
{
    string Authenticate(DeviceRegisterDto deviceRegisterDto, RequestContext requestContext);
    Device GetDevice(string deviceHash);
    public bool IsAuthenticated(string deviceHash) => GetDevice(deviceHash) != null;
    
    void Logout(Device device);
}