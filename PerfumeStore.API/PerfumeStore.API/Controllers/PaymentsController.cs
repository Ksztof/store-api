using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.Shared.DTO.Request.Order;
using PerfumeStore.API.Shared.DTO.Request.Payments;
using PerfumeStore.API.Shared.Extensions;
using PerfumeStore.API.Validators;
using PerfumeStore.API.Validators.Payments.PayWithCardDto;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Abstractions.Result.Shared;
using PerfumeStore.Application.Orders;
using PerfumeStore.Application.Payments;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using Stripe;

namespace PerfumeStore.API.Controllers
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
        public async Task<IActionResult> StartOrder([FromBody] StartOrderDtoApi request)
        {
            var validationResult = await _validationService.ValidateAsync(request);
            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();

            StartOrderDtoApp createOrderDtoApp = _mapper.Map<StartOrderDtoApp>(request);

            Result<PaymentIntent> result = await _paymentsService.StartOrderAsync(createOrderDtoApp);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
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
