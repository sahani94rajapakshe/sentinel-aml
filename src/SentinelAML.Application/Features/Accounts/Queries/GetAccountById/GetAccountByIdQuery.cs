using MediatR;
using SentinelAML.Application.Common.Models;
using SentinelAML.Application.Features.Accounts.Dtos;

namespace SentinelAML.Application.Features.Accounts.Queries.GetAccountById;

public record GetAccountByIdQuery(Guid Id) : IRequest<Result<AccountDto>>;
