using Firework.Server.Dto.Commands;
using FluentValidation;

namespace Firework.Server.Validators;

public sealed class RpcCommandValidator : AbstractValidator<RpcCommandDto>
{
    public RpcCommandValidator()
    {
        RuleFor(x => x.ModuleName)
            .NotEmpty().WithMessage("ModuleName is required.");

        RuleFor(x => x.ActionName)
            .NotEmpty().WithMessage("ActionName is required.");
    }
}

