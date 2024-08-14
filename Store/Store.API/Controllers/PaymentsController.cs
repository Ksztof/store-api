using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Shared.DTO.Request.Payments;
using Store.API.Shared.Extensions;
using Store.API.Validators;
using Store.Application.Payments;
using Store.Application.Shared.DTO.Request;
using Store.Domain.Abstractions;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IValidationService _validationService;
        private readonly IPaymentsService _paymentsService;
        private readonly IMapper _mapper;

        public PaymentsController(
            IMapper mapper,
            IValidationService validationService,
            IPaymentsService paymentsService)
        {
            _mapper = mapper;
            _validationService = validationService;
            _paymentsService = paymentsService;
        }

        [HttpPost]
        public async Task<IActionResult> GetClientSecret([FromBody] GetClientSecretDtoApi request)
        {
            var validationResult = await _validationService.ValidateAsync(request);
            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();

            GetClientSecretDtoApp getClientSecretDtoApp = _mapper.Map<GetClientSecretDtoApp>(request);

            Result<string> result = await _paymentsService.GetClientSecretAsync(getClientSecretDtoApp);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
        }

        [HttpPost("update-payment-intent")]
        public async Task<IActionResult> UpdatePaymentIntent([FromBody] UpdatePaymentIntentDtoApi request)
        {
            var validationResult = await _validationService.ValidateAsync(request);
            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();

            UpdatePaymentIntentDtoApp updatePaymentIntentDtoApp = _mapper.Map<UpdatePaymentIntentDtoApp>(request);

            Result result = await _paymentsService.UpdatePaymentIntentAsync(updatePaymentIntentDtoApp);

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDtoApi request)
        {
            var validationResult = await _validationService.ValidateAsync(request);
            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();

            ConfirmPaymentDtoApp createOrderDtoApp = _mapper.Map<ConfirmPaymentDtoApp>(request);

            Result result = await _paymentsService.ConfirmPaymentAsync(createOrderDtoApp);

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task StripeWebhook()
        {
            await _paymentsService.VerifyPaymentAsync();
        }
    }
}
