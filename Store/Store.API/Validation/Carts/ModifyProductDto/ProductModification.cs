﻿using FluentValidation;
using Store.API.Shared.DTO.Models;

namespace Store.API.Validation.Carts.ModifyProductDto;

internal class ProductModification : AbstractValidator<ProductModificationApi>
{
    internal ProductModification()
    {
        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Product ID must be greater than 0.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}
