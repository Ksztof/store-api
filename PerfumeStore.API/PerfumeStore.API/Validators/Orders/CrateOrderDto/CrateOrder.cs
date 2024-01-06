using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.Order;
using PerfumeStore.API.Validators.Carts.AddProductsToCartDto;

namespace PerfumeStore.API.Validators.Orders.CrateOrderDto
{
    public sealed class CrateOrder : AbstractValidator<CrateOrderDtoApi>
    {
        public CrateOrder()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.")
                                      .Matches("^[A-Z][a-zA-Z]*$").WithMessage("First name must start with a capital letter.");

            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.")
                                    .Matches("^[A-Z][a-zA-Z]*$").WithMessage("Last name must start with a capital letter.");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                                 .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.")
                                   .Matches("^[A-Z]").WithMessage("Street must start with a capital letter.");

            RuleFor(x => x.StreetNumber).NotEmpty().WithMessage("Street number is required.")
                                        .Matches(@"^\d+[a-zA-Z]*$").WithMessage("Street number must be a valid number with optional letter.");

            RuleFor(x => x.HomeNumber).NotEmpty().WithMessage("Home number is required.");

            RuleFor(x => x.PostCode).NotEmpty().WithMessage("Post code is required.")
                                    .Matches(@"^\d{2}-\d{3}$").WithMessage("Post code must be in the format XX-XXX.");

            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.")
                                .Matches("^[A-Z]").WithMessage("City must start with a capital letter.");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required.")
                                       .Matches(@"^\d{3}-\d{3}-\d{3}$").WithMessage("Phone number must be in the format XXX-XXX-XXX.");
        }
    }
}
