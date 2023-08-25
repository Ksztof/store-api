using FluentValidation.Results;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
