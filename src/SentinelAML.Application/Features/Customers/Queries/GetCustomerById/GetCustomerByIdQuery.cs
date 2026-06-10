using MediatR;
using SentinelAML.Application.Common.Models;
using SentinelAML.Application.Features.Customers.Dtos;

namespace SentinelAML.Application.Features.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;
