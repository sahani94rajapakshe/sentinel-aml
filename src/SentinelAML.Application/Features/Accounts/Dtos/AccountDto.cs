using SentinelAML.Domain.Entities;
using SentinelAML.Domain.Enums;

namespace SentinelAML.Application.Features.Accounts.Dtos;

public class AccountDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public AccountStatus Status { get; init; }

    public static AccountDto FromEntity(Account account) => new()
    {
        Id = account.Id,
        CustomerId = account.CustomerId,
        AccountNumber = account.AccountNumber,
        Balance = account.Balance,
        Status = account.Status
    };
}
