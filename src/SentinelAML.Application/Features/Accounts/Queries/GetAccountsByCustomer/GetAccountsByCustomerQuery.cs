using MediatR;
using SentinelAML.Application.Common.Models;
using SentinelAML.Application.Features.Accounts.Dtos;

namespace SentinelAML.Application.Features.Accounts.Queries.GetAccountsByCustomer;

public record GetAccountsByCustomerQuery(Guid CustomerId) : IRequest<Result<IReadOnlyList<AccountDto>>>;
