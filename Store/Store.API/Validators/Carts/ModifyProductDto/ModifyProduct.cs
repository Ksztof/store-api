using FluentValidation;
using Store.API.Shared.DTO.Request.Cart;

namespace Store.API.Validators.Carts.ModifyProductDto
{
    public sealed class ModifyProduct : AbstractValidator<ModifyProductDtoApi>
    {
        public ModifyProduct()
        {
            RuleFor(x => x.Product).NotNull().WithMessage("Product details are required.");
            RuleFor(x => x.Product).SetValidator(new ProductModification());
        }
    }
}
