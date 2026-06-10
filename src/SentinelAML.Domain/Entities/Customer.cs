using SentinelAML.Domain.Common;

namespace SentinelAML.Domain.Entities;

public class Customer : BaseEntity
{
    private readonly List<Account> _accounts = [];

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;

    public IReadOnlyCollection<Account> Accounts => _accounts.AsReadOnly();

    private Customer()
    {
    }

    public Customer(string firstName, string lastName, string email, string phoneNumber)
    {
        UpdateProfile(firstName, lastName, email, phoneNumber);
    }

    public void UpdateProfile(string firstName, string lastName, string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim();
        PhoneNumber = phoneNumber.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public Account OpenAccount(string accountNumber, decimal initialBalance = 0m)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException("Account number is required.", nameof(accountNumber));
        }

        if (initialBalance < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative.");
        }

        var account = new Account(Id, accountNumber.Trim(), initialBalance);
        _accounts.Add(account);

        return account;
    }
}
