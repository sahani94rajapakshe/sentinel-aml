using MediatR;
using SentinelAML.Application.Common.Models;

namespace SentinelAML.Application.Features.Accounts.Commands.CloseAccount;

public record CloseAccountCommand(Guid Id) : IRequest<Result>;
