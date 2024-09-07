using FluentValidation;
using Store.API.Shared.DTO.Request.Cart;

namespace Store.API.Validation.Carts.NewProductsDto;

internal class CheckCurrentCart : AbstractValidator<CheckCurrentCartDtoApi>
{
    internal CheckCurrentCart()
    {
        RuleFor(x => x.CreatedAt)
                    .Must(ValidationUtils.BeAValidUtcDateTime)
                    .WithMessage("CreatedAt must be a valid UTC date.");
    }
}
