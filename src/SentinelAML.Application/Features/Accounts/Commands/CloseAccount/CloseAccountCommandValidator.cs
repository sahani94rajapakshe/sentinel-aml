using FluentValidation;

namespace SentinelAML.Application.Features.Accounts.Commands.CloseAccount;

public class CloseAccountCommandValidator : AbstractValidator<CloseAccountCommand>
{
    public CloseAccountCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
