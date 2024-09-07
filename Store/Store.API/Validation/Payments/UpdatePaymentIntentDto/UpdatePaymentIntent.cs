using FluentValidation;
using Store.API.Shared.DTO.Request.Payments;

namespace Store.API.Validation.Payments.UpdatePaymentIntentDto;

internal class UpdatePaymentIntent : AbstractValidator<UpdatePaymentIntentDtoApi>
{
    internal UpdatePaymentIntent()
    {
        RuleFor(x => x.clientSecret)
            .NotEmpty().WithMessage("Client secret cannot be empty")
        .Matches(@"^pi_[a-zA-Z0-9]+_secret_[a-zA-Z0-9]+$").WithMessage("Client secret must be in the format 'pi_XXXXXXXXXXXXXXXX_secret_XXXXXXXXXXXXXXXX'.");
    }
}
