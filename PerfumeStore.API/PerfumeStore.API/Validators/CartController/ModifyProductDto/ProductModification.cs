﻿using FluentValidation;
using PerfumeStore.API.Shared.DTO.Models;

namespace PerfumeStore.API.Validators.CartController.ModifyProductDto
{
    public class ProductModification : AbstractValidator<ProductModificationApi>
    {
        public ProductModification()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Product ID must be greater than 0.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
