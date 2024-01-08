using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.Cart;

namespace PerfumeStore.API.Validators.Carts.ModifyProductDto
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
