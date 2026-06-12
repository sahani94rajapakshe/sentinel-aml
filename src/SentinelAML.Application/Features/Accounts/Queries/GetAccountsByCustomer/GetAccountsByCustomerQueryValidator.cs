using FluentValidation;

namespace SentinelAML.Application.Features.Accounts.Queries.GetAccountsByCustomer;

public class GetAccountsByCustomerQueryValidator : AbstractValidator<GetAccountsByCustomerQuery>
{
    public GetAccountsByCustomerQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}
