using Contracts.Enums;

namespace Api.ViewModels.V1;

public class AddOrderInVm
{
    public long CustomerId { get; set; }

    public OrderSideContract Side { get; set; }

    public decimal Amount { get; set; }
}
