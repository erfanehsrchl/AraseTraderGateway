namespace Api.ViewModels.V1;

public class WalletTransactionOutVm
{
    public long Id { get; set; }

    public long WalletId { get; set; }

    public long? OrderId { get; set; }

    public decimal Amount { get; set; }

    public decimal BalanceBefore { get; set; }

    public decimal BalanceAfter { get; set; }

    public int Reason { get; set; }

    public DateTime CreatedAt { get; set; }
}
