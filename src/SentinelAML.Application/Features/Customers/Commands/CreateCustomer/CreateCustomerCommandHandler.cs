using MediatR;
using SentinelAML.Application.Common.Interfaces;
using SentinelAML.Application.Common.Models;
using SentinelAML.Domain.Entities;

namespace SentinelAML.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        IRepository<Customer> customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var existingCustomers = await _customerRepository.FindAsync(
            c => c.Email == email,
            cancellationToken);

        if (existingCustomers.Count > 0)
        {
            return Result<Guid>.Failure("A customer with this email already exists.");
        }

        var customer = new Customer(
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(customer.Id);
    }
}
