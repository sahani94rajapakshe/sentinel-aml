using SentinelAML.Domain.Common;
using SentinelAML.Domain.Enums;

namespace SentinelAML.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid FromAccountId { get; private set; }
    public Guid ToAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public int RiskScore { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime TransactionDate { get; private set; }

    public Account FromAccount { get; private set; } = null!;
    public Account ToAccount { get; private set; } = null!;

    private Transaction()
    {
    }

    public Transaction(
        Account fromAccount,
        Account toAccount,
        decimal amount,
        DateTime transactionDate)
    {
        ArgumentNullException.ThrowIfNull(fromAccount);
        ArgumentNullException.ThrowIfNull(toAccount);

        if (fromAccount.Id == toAccount.Id)
        {
            throw new InvalidOperationException("Source and destination accounts must be different.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Transaction amount must be greater than zero.");
        }

        FromAccountId = fromAccount.Id;
        ToAccountId = toAccount.Id;
        Amount = amount;
        RiskScore = 0;
        Status = TransactionStatus.Pending;
        TransactionDate = transactionDate;

        fromAccount.RegisterOutgoingTransaction(this);
        toAccount.RegisterIncomingTransaction(this);
    }

    public void Complete(int riskScore)
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new InvalidOperationException("Only pending transactions can be completed.");
        }

        if (riskScore < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(riskScore), "Risk score cannot be negative.");
        }

        RiskScore = riskScore;
        Status = TransactionStatus.Completed;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Fail()
    {
        if (Status != TransactionStatus.Pending)
        {
            throw new InvalidOperationException("Only pending transactions can be marked as failed.");
        }

        Status = TransactionStatus.Failed;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
