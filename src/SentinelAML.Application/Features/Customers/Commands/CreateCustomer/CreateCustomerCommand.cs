using MediatR;
using SentinelAML.Application.Common.Models;

namespace SentinelAML.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<Result<Guid>>
{
    public CreateCustomerCommand()
    {
    }

    public CreateCustomerCommand(CreateCustomerDto dto)
    {
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        Email = dto.Email;
        PhoneNumber = dto.PhoneNumber;
    }

    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
