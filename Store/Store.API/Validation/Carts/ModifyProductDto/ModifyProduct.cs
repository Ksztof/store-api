using FluentValidation;
using Store.API.Shared.DTO.Request.Cart;

namespace Store.API.Validation.Carts.ModifyProductDto;

internal sealed class ModifyProduct : AbstractValidator<ModifyProductDtoApi>
{
    internal ModifyProduct()
    {
        RuleFor(x => x.Product).NotNull().WithMessage("Product details are required.");
        RuleFor(x => x.Product).SetValidator(new ProductModification());
    }
}
