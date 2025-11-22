using Firework.Server.Dto.Register;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Firework.Server.Abstraction;

public interface IRegistrationService
{
    Task<Result<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request, HttpContext httpContext, CancellationToken cancellationToken = default);
}

