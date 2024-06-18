using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.Shared.DTO.Request.Order;
using PerfumeStore.API.Shared.DTO.Request.Payments;
using PerfumeStore.API.Shared.Extensions;
using PerfumeStore.API.Validators;
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
        public async Task<IActionResult> PayWithCard([FromBody] PayWithCardDtoApi request)
        {
            var validationResult = await _validationService.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            PayWithCardDtoApp createOrderDtoApp = _mapper.Map<PayWithCardDtoApp>(request);

            Result result = await _paymentsService.PayWithCardAsync(createOrderDtoApp);

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
