using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.Product;

namespace PerfumeStore.API.Validators.Products.CreateProductDto
{
    public sealed class CreateProduct : AbstractValidator<CreateProductDtoApi>
    {
        public CreateProduct()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.");

            RuleFor(x => x.ProductPrice)
                .NotEmpty().WithMessage("Product price is required.")
                .GreaterThan(0.01m).WithMessage("Product price must be greater than one cent.")
                .LessThan(10000000m).WithMessage("Product price must be less than ten million.");

            RuleFor(x => x.ProductCategoriesIds)
                .Must(BeValidCategoryIds).WithMessage("All category IDs must be greater than zero.");

            RuleFor(x => x.ProductManufacturer)
                .NotEmpty().WithMessage("Product manufacturer is required.");
        }

        private bool BeValidCategoryIds(ICollection<int> categoryIds)
        {
            return categoryIds.All(id => id > 0);
        }
    }
}
