using MediatR;
using SentinelAML.Application.Common.Interfaces;
using SentinelAML.Application.Common.Models;
using SentinelAML.Application.Features.Accounts.Dtos;
using SentinelAML.Domain.Entities;

namespace SentinelAML.Application.Features.Accounts.Queries.GetAccountById;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, Result<AccountDto>>
{
    private readonly IRepository<Account> _accountRepository;

    public GetAccountByIdQueryHandler(IRepository<Account> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<AccountDto>> Handle(
        GetAccountByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result<AccountDto>.Failure("Account id is required.");
        }

        var account = await _accountRepository.GetByIdAsync(request.Id, cancellationToken);

        if (account is null)
        {
            return Result<AccountDto>.Failure("Account not found.");
        }

        return Result<AccountDto>.Success(AccountDto.FromEntity(account));
    }
}
