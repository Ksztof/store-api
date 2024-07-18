using FluentValidation;
using PerfumeStore.API.Shared.DTO.Request.Payments;

namespace PerfumeStore.API.Validators.Payments.UpdatePaymentIntentDto
{
    public class UpdatePaymentIntent : AbstractValidator<UpdatePaymentIntentDtoApi>
    {
        public UpdatePaymentIntent()
        {
            RuleFor(x => x.PaymentIntentId)
                .NotEmpty().WithMessage("Payment intent Id cannot be empty")
                .Matches(@"^pi_\w+$").WithMessage("Payment intent Id must start with 'pi_' and contain only alphanumeric characters.");
        }
    }
}
