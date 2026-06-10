using MediatR;
using SentinelAML.Application.Common.Models;
using SentinelAML.Application.Features.Customers.Dtos;

namespace SentinelAML.Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQuery : IRequest<Result<IReadOnlyList<CustomerDto>>>
{
}
