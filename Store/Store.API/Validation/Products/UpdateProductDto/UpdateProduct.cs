﻿using FluentValidation;
using Store.API.Shared.DTO.Request.Product;

namespace Store.API.Validation.Products.UpdateProductDto;

internal sealed class UpdateProduct : AbstractValidator<UpdateProductDtoApi>
{
    internal UpdateProduct()
    {
        RuleFor(x => x.productId)
            .GreaterThan(0).WithMessage("Product ID must be greater than zero.");

        RuleFor(x => x.ProductPrice)
            .GreaterThan(0.01m).WithMessage("Product price must be greater than one gr.")
            .LessThan(10000000m).WithMessage("Product price must be less than ten million.")
            .When(x => x.ProductPrice > 0);

        RuleForEach(x => x.ProductCategoriesIds)
            .Must(ValidationUtils.BeValidCategoryId).WithMessage("All category IDs must be greater than zero.")
            .When(x => x.ProductCategoriesIds != null && x.ProductCategoriesIds.Any());

        RuleFor(x => x.ProductManufacturer)
            .MaximumLength(100).WithMessage("The field must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.ProductManufacturer));

        RuleFor(x => x.ProductDescription)
            .MaximumLength(500).WithMessage("The field must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.ProductDescription));

        RuleFor(x => x.ProductName)
            .MaximumLength(100).WithMessage("The field must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.ProductDescription));
    }
}
