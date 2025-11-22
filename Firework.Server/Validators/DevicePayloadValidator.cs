using System.Net;
using Firework.Server.Dto.Devices;
using FluentValidation;

namespace Firework.Server.Validators;

public sealed class DevicePayloadValidator : AbstractValidator<DevicePayloadDto>
{
    public DevicePayloadValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty().WithMessage("DeviceId must be provided.");

        RuleFor(x => x.DeviceName)
            .NotEmpty().WithMessage("DeviceName is required.")
            .MaximumLength(128).WithMessage("DeviceName is too long.");

        RuleFor(x => x.Ip)
            .NotEmpty().WithMessage("IP address is required.")
            .Must(IsValidIp).WithMessage("IP address is invalid.");
    }

    private static bool IsValidIp(string ip)
    {
        return IPAddress.TryParse(ip, out _);
    }
}

