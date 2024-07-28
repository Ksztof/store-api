using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.Cart;

namespace PerfumeStore.API.Validators.Carts.NewProductsDto
{
    public class IsCurrentCart : AbstractValidator<IsCurrentCartDtoApi>
    {
        public IsCurrentCart()
        {
            RuleFor(x => x.CartId)
                .GreaterThan(0)
                .WithMessage("Cart ID must be greater than 0.");

            RuleFor(x => x.CreatedAt)
                        .Must(ValidationUtils.BeAValidUtcDateTime)
                        .WithMessage("CreatedAt must be a valid UTC date.");
        }
    }
}
