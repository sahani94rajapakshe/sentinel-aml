using MediatR;
using SentinelAML.Application.Common.Interfaces;
using SentinelAML.Application.Common.Models;
using SentinelAML.Application.Features.Customers.Dtos;
using SentinelAML.Domain.Entities;

namespace SentinelAML.Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<IReadOnlyList<CustomerDto>>>
{
    private readonly IRepository<Customer> _customerRepository;

    public GetCustomersQueryHandler(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<IReadOnlyList<CustomerDto>>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);

        var customerDtos = customers
            .Select(CustomerDto.FromEntity)
            .ToList();

        return Result<IReadOnlyList<CustomerDto>>.Success(customerDtos);
    }
}
