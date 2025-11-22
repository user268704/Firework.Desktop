using Firework.Server.Dto.Commands.Parameters;
using FluentValidation;

namespace Firework.Server.Validators;

public sealed class EchoCommandParamsValidator : AbstractValidator<EchoCommandParams>
{
    public EchoCommandParamsValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(512).WithMessage("Message is too long.");
    }
}

