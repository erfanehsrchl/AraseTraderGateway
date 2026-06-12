using Api.ViewModels.V1;
using Contracts.Enums;
using FluentValidation;

namespace Api.Validators.V1;

public class AddOrderInVmValidator : AbstractValidator<AddOrderInVm>
{
    public AddOrderInVmValidator()
    {
        RuleFor(request => request.CustomerId)
            .GreaterThan(0);

        RuleFor(request => request.Side)
            .Must(side => Enum.IsDefined(typeof(OrderSideContract), side));

        RuleFor(request => request.Amount)
            .GreaterThan(0);
    }
}
