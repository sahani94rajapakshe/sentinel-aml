using MediatR;
using SentinelAML.Application.Common.Interfaces;
using SentinelAML.Application.Common.Models;
using SentinelAML.Application.Features.Accounts.Dtos;
using SentinelAML.Domain.Entities;

namespace SentinelAML.Application.Features.Accounts.Queries.GetAccountsByCustomer;

public class GetAccountsByCustomerQueryHandler
    : IRequestHandler<GetAccountsByCustomerQuery, Result<IReadOnlyList<AccountDto>>>
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IRepository<Customer> _customerRepository;

    public GetAccountsByCustomerQueryHandler(
        IRepository<Account> accountRepository,
        IRepository<Customer> customerRepository)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<IReadOnlyList<AccountDto>>> Handle(
        GetAccountsByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CustomerId == Guid.Empty)
        {
            return Result<IReadOnlyList<AccountDto>>.Failure("Customer id is required.");
        }

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result<IReadOnlyList<AccountDto>>.Failure("Customer not found.");
        }

        var accounts = await _accountRepository.FindAsync(
            a => a.CustomerId == request.CustomerId,
            cancellationToken);

        var accountDtos = accounts
            .Select(AccountDto.FromEntity)
            .ToList();

        return Result<IReadOnlyList<AccountDto>>.Success(accountDtos);
    }
}
