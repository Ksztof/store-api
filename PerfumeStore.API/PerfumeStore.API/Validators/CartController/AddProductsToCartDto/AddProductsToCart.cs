using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PerfumeStore.API.Validators.CartController.AddProductsToCartDto
{
    public sealed class AddProductsToCart : AbstractValidator<AddProductsToCartDtoApi>
    {
        public AddProductsToCart()
        {
            RuleFor(x => x.Products).NotEmpty().WithMessage("The products list cannot be empty.");
            RuleForEach(x => x.Products).SetValidator(new ProductInCart());
        }
    }
}
