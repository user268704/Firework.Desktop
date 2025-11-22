using Firework.Server.Dto.Commands.Parameters;
using FluentValidation;

namespace Firework.Server.Validators;

public sealed class SystemInfoParamsValidator : AbstractValidator<SystemInfoParams>
{
    public SystemInfoParamsValidator()
    {
        RuleFor(x => x.EnvironmentVariableLimit)
            .GreaterThan(0).WithMessage("EnvironmentVariableLimit must be positive.")
            .LessThanOrEqualTo(50).WithMessage("EnvironmentVariableLimit is too large.");
    }
}

