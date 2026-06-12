using MediatR;
using SentinelAML.Application.Common.Interfaces;
using SentinelAML.Application.Common.Models;
using SentinelAML.Domain.Entities;

namespace SentinelAML.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result<Guid>>
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountCommandHandler(
        IRepository<Account> accountRepository,
        IRepository<Customer> customerRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateAccountCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result<Guid>.Failure("Customer not found.");
        }

        var accountNumber = request.AccountNumber.Trim();

        var existingAccounts = await _accountRepository.FindAsync(
            a => a.AccountNumber == accountNumber,
            cancellationToken);

        if (existingAccounts.Count > 0)
        {
            return Result<Guid>.Failure("An account with this account number already exists.");
        }

        var account = Account.Create(
            request.CustomerId,
            accountNumber,
            request.Balance);

        await _accountRepository.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(account.Id);
    }
}
