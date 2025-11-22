using Firework.Server.Dto.Register;
using FluentValidation;

namespace Firework.Server.Validators;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.AccessCode)
            .NotEmpty().WithMessage("AccessCode is required.");

        RuleFor(x => x.Payload)
            .SetValidator(new DevicePayloadValidator());
    }
}

