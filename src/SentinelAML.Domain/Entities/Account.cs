using SentinelAML.Domain.Common;
using SentinelAML.Domain.Enums;

namespace SentinelAML.Domain.Entities;

public class Account : BaseEntity
{
    private readonly List<Transaction> _outgoingTransactions = [];
    private readonly List<Transaction> _incomingTransactions = [];

    public Guid CustomerId { get; private set; }
    public string AccountNumber { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public AccountStatus Status { get; private set; }

    public Customer Customer { get; private set; } = null!;
    public IReadOnlyCollection<Transaction> OutgoingTransactions => _outgoingTransactions.AsReadOnly();
    public IReadOnlyCollection<Transaction> IncomingTransactions => _incomingTransactions.AsReadOnly();

    private Account()
    {
    }

    public static Account Create(Guid customerId, string accountNumber, decimal initialBalance = 0m)
    {
        return new Account(customerId, accountNumber, initialBalance);
    }

    internal Account(Guid customerId, string accountNumber, decimal initialBalance)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException("Customer identifier is required.", nameof(customerId));
        }

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException("Account number is required.", nameof(accountNumber));
        }

        if (initialBalance < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialBalance), "Balance cannot be negative.");
        }

        CustomerId = customerId;
        AccountNumber = accountNumber;
        Balance = initialBalance;
        Status = AccountStatus.Active;
    }

    public void Suspend()
    {
        EnsureNotClosed();

        if (Status == AccountStatus.Suspended)
        {
            return;
        }

        Status = AccountStatus.Suspended;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == AccountStatus.Closed)
        {
            throw new InvalidOperationException("A closed account cannot be reactivated.");
        }

        if (Status == AccountStatus.Active)
        {
            return;
        }

        Status = AccountStatus.Active;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Close()
    {
        if (Status == AccountStatus.Closed)
        {
            return;
        }

        if (Balance != 0)
        {
            throw new InvalidOperationException("Account balance must be zero before closing.");
        }

        Status = AccountStatus.Closed;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Credit(decimal amount)
    {
        EnsureActive();

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Credit amount must be greater than zero.");
        }

        Balance += amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Debit(decimal amount)
    {
        EnsureActive();

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Debit amount must be greater than zero.");
        }

        if (Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }

        Balance -= amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void RegisterOutgoingTransaction(Transaction transaction)
    {
        _outgoingTransactions.Add(transaction);
    }

    internal void RegisterIncomingTransaction(Transaction transaction)
    {
        _incomingTransactions.Add(transaction);
    }

    private void EnsureActive()
    {
        if (Status != AccountStatus.Active)
        {
            throw new InvalidOperationException($"Account {AccountNumber} is not active.");
        }
    }

    private void EnsureNotClosed()
    {
        if (Status == AccountStatus.Closed)
        {
            throw new InvalidOperationException($"Account {AccountNumber} is closed.");
        }
    }
}
