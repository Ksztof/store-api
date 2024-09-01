using FluentValidation;
using Store.API.Shared.DTO.Request.Cart;

namespace Store.API.Validation.Carts.AddProductsToCartDto
{
    public sealed class AddProductsToCart : AbstractValidator<NewProductsDtoApi>
    {
        public AddProductsToCart()
        {
            RuleFor(x => x.Products).NotEmpty().WithMessage("The products list cannot be empty.");
            RuleForEach(x => x.Products).SetValidator(new ProductInCart());
        }
    }
}
