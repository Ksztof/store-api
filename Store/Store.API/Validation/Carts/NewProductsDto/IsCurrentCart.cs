using FluentValidation;
using Store.API.Shared.DTO.Request.Cart;

namespace Store.API.Validation.Carts.NewProductsDto;

internal class IsCurrentCart : AbstractValidator<IsCurrentCartDtoApi>
{
    internal IsCurrentCart()
    {
        RuleFor(x => x.CartId)
            .GreaterThan(0)
            .WithMessage("Cart ID must be greater than 0.");

        RuleFor(x => x.CreatedAt)
                    .Must(ValidationUtils.BeAValidUtcDateTime)
                    .WithMessage("CreatedAt must be a valid UTC date.");
    }
}
