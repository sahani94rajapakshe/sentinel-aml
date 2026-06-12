using FluentValidation;

namespace SentinelAML.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Balance)
            .GreaterThanOrEqualTo(0);
    }
}
