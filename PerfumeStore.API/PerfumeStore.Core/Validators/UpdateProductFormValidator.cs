﻿using FluentValidation;
using PerfumeStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfumeStore.Core.Validators
{
    public class UpdateProductFormValidator : AbstractValidator<UpdateProductForm>
    {
        public UpdateProductFormValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .Length(0, 100).WithMessage("Product name must be less than 100 characters.");

            RuleFor(x => x.ProductPrice)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0).WithMessage("Product price must be greater than 0.")
                .Must(HasMaximumTwoDecimalPlaces)
                .WithMessage("Product price can have up to 2 decimal places.");

            RuleFor(x => x.ProductDescription)
                .MaximumLength(2000).WithMessage("Product description must be less than 2000 characters.");

            RuleFor(x => x.ProductCategoriesIds)
                .NotEmpty().WithMessage("At least one category ID is required.");

            RuleFor(x => x.ProductManufacturer)
                .MaximumLength(200).WithMessage("Product manufacturer must be less than 200 characters.");
        }

        private bool HasMaximumTwoDecimalPlaces(decimal value)
        {
            decimal valueTimes100 = value * 100;
            return Math.Floor(valueTimes100) == valueTimes100;
        }
    }
}