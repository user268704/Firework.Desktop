using Firework.Server.Abstraction;
using Firework.Server.Dto.Devices;
using Firework.Server.Dto.Register;
using Firework.Server.Models.Clients;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Firework.Server.Services;

public sealed class RegistrationService : IRegistrationService
{
    private readonly IValidator<RegisterRequestDto> _validator;
    private readonly IAccessCodeService _accessCodeService;
    private readonly IClientRegistry _clientRegistry;
    private readonly ILogger<RegistrationService> _logger;

    public RegistrationService(
        IValidator<RegisterRequestDto> validator,
        IAccessCodeService accessCodeService,
        IClientRegistry clientRegistry,
        ILogger<RegistrationService> logger)
    {
        _validator = validator;
        _accessCodeService = accessCodeService;
        _clientRegistry = clientRegistry;
        _logger = logger;
    }

    public async Task<Result<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request, HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            return Result.Fail(validation.ToString());
        }

        var codeResult = _accessCodeService.EnsureValid(request.AccessCode);

        if (codeResult.IsFailed)
        {
            return codeResult.ToResult<RegisterResponseDto>();
        }

        var ip = request.Payload.Ip;

        if (string.IsNullOrWhiteSpace(ip) && httpContext.Connection.RemoteIpAddress != null)
        {
            ip = httpContext.Connection.RemoteIpAddress.ToString();
        }

        var payload = new DevicePayloadDto
        {
            DeviceId = request.Payload.DeviceId,
            DeviceName = request.Payload.DeviceName,
            Ip = ip
        };

        var registerResult = _clientRegistry.Register(payload, ip);

        if (registerResult.IsFailed)
        {
            return registerResult.ToResult<RegisterResponseDto>();
        }

        var rotateResult = await _accessCodeService.RotateAsync(cancellationToken);

        if (rotateResult.IsFailed)
        {
            return rotateResult.ToResult<RegisterResponseDto>();
        }

        var client = registerResult.Value;
        _logger.LogInformation("Client {Device} registered from {Ip}.", client.DeviceName, client.Ip);

        var response = new RegisterResponseDto
        {
            Token = client.Token,
            DeviceId = client.DeviceId,
            DeviceName = client.DeviceName,
            NextAccessCode = rotateResult.Value
        };

        return Result.Ok(response);
    }
}

