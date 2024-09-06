using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Shared.DTO.Request.Payments;
using Store.API.Shared.Extensions.Models;
using Store.API.Shared.Extensions.Results;
using Store.API.Validation;
using Store.Application.Payments;
using Store.Application.Payments.Dto.Request;
using Store.Domain.Abstractions;

namespace Store.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    [AllowAnonymous]
    public async Task<IActionResult> GetClientSecretAsync([FromBody] GetClientSecretDtoApi request)
    {
        var validationResult = await _validationService.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        GetClientSecretDtoApp getClientSecretDtoApp = _mapper.Map<GetClientSecretDtoApp>(request);

        Result<string> result = await _paymentsService.GetClientSecretAsync(getClientSecretDtoApp);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails();
    }

    [HttpPost("update-payment-intent")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdatePaymentIntentAsync([FromBody] UpdatePaymentIntentDtoApi request)
    {
        var validationResult = await _validationService.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        UpdatePaymentIntentDtoApp updatePaymentIntentDtoApp = _mapper.Map<UpdatePaymentIntentDtoApp>(request);

        Result result = await _paymentsService.UpdatePaymentIntentAsync(updatePaymentIntentDtoApp);

        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    [HttpPost("confirm-payment")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmPaymentAsync([FromBody] ConfirmPaymentDtoApi request)
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
