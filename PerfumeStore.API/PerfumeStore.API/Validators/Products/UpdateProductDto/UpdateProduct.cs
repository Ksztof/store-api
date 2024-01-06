using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.Product;

namespace PerfumeStore.API.Validators.Products.UpdateProductDto
{
    public sealed class UpdateProduct : AbstractValidator<UpdateProductDtoApi>
    {
        public UpdateProduct()
        {
            RuleFor(x => x.productId)
                .GreaterThan(0).WithMessage("Product ID must be greater than zero.");

            When(x => x.ProductPrice > 0, () =>
            {
                RuleFor(x => x.ProductPrice)
                    .GreaterThan(0.01m).WithMessage("Product price must be greater than one gr.")
                    .LessThan(10000000m).WithMessage("Product price must be less than ten million.");
            });


            When(x => x.ProductCategoriesIds != null && x.ProductCategoriesIds.Any(), () =>
            {
                RuleFor(x => x.ProductCategoriesIds)
                    .Must(BeValidCategoryIds).WithMessage("All category IDs must be greater than zero.");
            });
        }

        private bool BeValidCategoryIds(ICollection<int> categoryIds)
        {
            return categoryIds.All(id => id > 0);
        }
    }
}
