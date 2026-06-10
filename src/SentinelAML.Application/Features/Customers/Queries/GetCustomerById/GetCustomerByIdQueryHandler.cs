using MediatR;
using SentinelAML.Application.Common.Interfaces;
using SentinelAML.Application.Common.Models;
using SentinelAML.Application.Features.Customers.Dtos;
using SentinelAML.Domain.Entities;

namespace SentinelAML.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;

    public GetCustomerByIdQueryHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            return Result<CustomerDto>.Failure("Customer id is required.");
        }

        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (customer is null)
        {
            return Result<CustomerDto>.Failure("Customer not found.");
        }

        return Result<CustomerDto>.Success(CustomerDto.FromEntity(customer));
    }
}
