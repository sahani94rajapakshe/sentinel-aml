using MediatR;
using SentinelAML.Application.Common.Interfaces;
using SentinelAML.Application.Common.Models;
using SentinelAML.Domain.Entities;
using SentinelAML.Domain.Enums;

namespace SentinelAML.Application.Features.Accounts.Commands.CloseAccount;

public class CloseAccountCommandHandler : IRequestHandler<CloseAccountCommand, Result>
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseAccountCommandHandler(
        IRepository<Account> accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CloseAccountCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.Id, cancellationToken);

        if (account is null)
        {
            return Result.Failure("Account not found.");
        }

        if (account.Status == AccountStatus.Closed)
        {
            return Result.Failure("Account is already closed.");
        }

        try
        {
            account.Close();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
