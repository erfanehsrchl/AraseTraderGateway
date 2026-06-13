namespace Api.ViewModels.V1;

public class GetWalletByCustomerIdOutVm
{
    public long WalletId { get; set; }

    public long CustomerId { get; set; }

    public decimal Balance { get; set; }
}
