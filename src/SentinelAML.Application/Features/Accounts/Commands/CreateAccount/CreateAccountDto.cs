namespace SentinelAML.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountDto
{
    public Guid CustomerId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
