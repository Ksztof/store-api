using FluentValidation;
using Store.API.Shared.DTO.Request.Product;

namespace Store.API.Validation.Products.CreateProductDto;

internal sealed class CreateProduct : AbstractValidator<CreateProductDtoApi>
{
    internal CreateProduct()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required.");

        RuleFor(x => x.ProductPrice)
            .NotEmpty().WithMessage("Product price is required.")
            .GreaterThan(0.01m).WithMessage("Product price must be greater than one cent.")
            .LessThan(10000000m).WithMessage("Product price must be less than ten million.");

        RuleForEach(x => x.ProductCategoriesIds)
                 .Must(ValidationUtils.BeValidCategoryId).WithMessage("All category IDs must be greater than zero.");

        RuleFor(x => x.ProductManufacturer)
            .NotEmpty().WithMessage("Product manufacturer is required.")
            .MaximumLength(100).WithMessage("The field must not exceed 100 characters.");
    }
}
