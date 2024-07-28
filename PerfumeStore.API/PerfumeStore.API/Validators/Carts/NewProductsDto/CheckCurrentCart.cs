using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.Cart;

namespace PerfumeStore.API.Validators.Carts.NewProductsDto
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
