using FluentValidation.Results;
using PerfumeStore.Core.Validators;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
  public class ValidationService : IValidationService
  {
    private readonly EntityIntIdValidator _entityIntIdValidator;
    private readonly QuantityValidator _quantityValidator;
    private readonly CreateProductFormValidator _createProductFormValidator;
    private readonly UpdateProductFormValidator _updateProductFormValidator;

    public ValidationService(EntityIntIdValidator entityIntIdValidator, QuantityValidator quantityValidator, CreateProductFormValidator createProductFormValidator, UpdateProductFormValidator updateProductFormValidator)
    {
      _entityIntIdValidator = entityIntIdValidator;
      _quantityValidator = quantityValidator;
      _createProductFormValidator = createProductFormValidator;
      _updateProductFormValidator = updateProductFormValidator;
    }
    public ValidationResult ValidateEntityId(int id)
    {
      return _entityIntIdValidator.Validate(id);
    }

    public ValidationResult ValidateQuantity(decimal quantity)
    {
      return _quantityValidator.Validate(quantity);
    }

    public ValidationResult ValidateCreateProductForm(CreateProductForm form)
    {
      return _createProductFormValidator.Validate(form);
    }

    public ValidationResult ValidateUpdateProductForm(UpdateProductForm form)
    {
      return _updateProductFormValidator.Validate(form);
    }
  }
}
