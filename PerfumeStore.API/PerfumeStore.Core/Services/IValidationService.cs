using FluentValidation.Results;
using PerfumeStore.Core.Models;

namespace PerfumeStore.Core.Services
{
    public interface IValidationService
    {
        ValidationResult ValidateEntityId(int id);
        ValidationResult ValidateQuantity(decimal quantity);
        ValidationResult ValidateCreateProductForm(CreateProductForm form);
        ValidationResult ValidateUpdateProductForm(UpdateProductForm form);
    }
}
