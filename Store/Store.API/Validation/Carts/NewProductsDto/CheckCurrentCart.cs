using FluentValidation;
using Store.API.Shared.DTO.Request.Cart;

namespace Store.API.Validation.Carts.NewProductsDto
{
    public class CheckCurrentCart : AbstractValidator<CheckCurrentCartDtoApi>
    {
        public CheckCurrentCart()
        {
            RuleFor(x => x.CreatedAt)
                        .Must(ValidationUtils.BeAValidUtcDateTime)
                        .WithMessage("CreatedAt must be a valid UTC date.");
        }
    }
}
