using Contracts.Enums;

namespace Application.DTOs;

public class AddOrderInDto
{
    public long CustomerId { get; set; }

    public OrderSideContract Side { get; set; }

    public decimal Amount { get; set; }
}
