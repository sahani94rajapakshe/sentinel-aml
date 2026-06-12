using MediatR;
using SentinelAML.Application.Common.Models;

namespace SentinelAML.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountCommand : IRequest<Result<Guid>>
{
    public CreateAccountCommand()
    {
    }

    public CreateAccountCommand(CreateAccountDto dto)
    {
        CustomerId = dto.CustomerId;
        AccountNumber = dto.AccountNumber;
        Balance = dto.Balance;
    }

    public Guid CustomerId { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Balance { get; init; }
}
