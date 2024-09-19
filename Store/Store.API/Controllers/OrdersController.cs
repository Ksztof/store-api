using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Shared.DTO.Request.Order;
using Store.API.Shared.Extensions.Models;
using Store.API.Shared.Extensions.Results;
using Store.API.Validation;
using Store.Application.Orders;
using Store.Application.Orders.Dto.Request;
using Store.Application.Orders.Dto.Response;
using Store.Domain.Abstractions;
using Store.Domain.StoreUsers.Roles;

namespace Store.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    public readonly IOrdersService _orderService;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;

    public OrdersController(
        IOrdersService orderService,
        IMapper mapper,
        IValidationService validationService)
    {
        _orderService = orderService;
        _mapper = mapper;
        _validationService = validationService;
    }

    [HttpPost("{method?}")]
    [AllowAnonymous]
    public async Task<IActionResult> SubmitOrderAsync(string? method, [FromBody] CrateOrderDtoApi createOrderRequest)
    {
        var dtoValidationResult = await _validationService.ValidateAsync(createOrderRequest);

        if (!dtoValidationResult.IsValid)
        {
            return dtoValidationResult.ToValidationProblemDetails();
        }

        if (!string.IsNullOrEmpty(method))
        {
            var parameterValidationResult = await _validationService.ValidateAsync(method);

            if (!parameterValidationResult.IsValid)
            {
                return parameterValidationResult.ToValidationProblemDetails();
            }
        }

        CreateOrderDtoApp createOrderDtoApp = _mapper.Map<CreateOrderDtoApp>(createOrderRequest);

        EntityResult<OrderResponseDto> result = await _orderService.SubmitOrderAsync(method, createOrderDtoApp);

        return result.IsSuccess ? Ok(result.Entity) : result.ToProblemDetails();
    }

    [HttpGet("{orderId}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> GetOrderByIdAsync(int orderId)
    {
        EntityResult<OrderResponseDto> result = await _orderService.GetOrderByIdAsync(orderId);

        return result.IsSuccess ? Ok(result.Entity) : result.ToProblemDetails();
    }

    [HttpDelete("{orderId}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> DeleteOrderAsync(int orderId)
    {
        EntityResult<OrderResponseDto> result = await _orderService.DeleteOrderAsync(orderId);

        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    [HttpPatch("{orderId}/mark-as-deleted")]
    [AllowAnonymous]
    public async Task<IActionResult> MarkOrderAsDeletedAsync(int orderId)
    {
        EntityResult<OrderResponseDto> result = await _orderService.MarkOrderAsDeletedAsync(orderId);

        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> GetOrdersAsync()
    {
        EntityResult<IEnumerable<OrdersResDto>> result = await _orderService.GetOrdersAsync();

        return result.IsSuccess ? Ok(result.Entity) : result.ToProblemDetails();
    }
}