using SentinelAML.Domain.Entities;

namespace SentinelAML.Application.Features.Customers.Dtos;

public class CustomerDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;

    public static CustomerDto FromEntity(Customer customer) => new()
    {
        Id = customer.Id,
        FirstName = customer.FirstName,
        LastName = customer.LastName,
        Email = customer.Email,
        PhoneNumber = customer.PhoneNumber
    };
}
