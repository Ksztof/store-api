using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.API.Shared.DTO.Request.Order;
using Store.API.Shared.Extensions;
using Store.API.Validators;
using Store.Application.Orders;
using Store.Application.Shared.DTO.Request;
using Store.Application.Shared.DTO.Response;
using Store.Domain.Abstractions;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public readonly IOrdersService _orderService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public OrdersController(IOrdersService orderService, IMapper mapper, IValidationService validationService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _validationService = validationService;
        }

        [HttpPost("{method?}")]
        public async Task<IActionResult> SubmitOrder(string? method, [FromBody] CrateOrderDtoApi createOrderRequest)
        {
            var dtoValidationResult = await _validationService.ValidateAsync(createOrderRequest);

            if (!dtoValidationResult.IsValid)
                return dtoValidationResult.ToValidationProblemDetails();

            if (!string.IsNullOrEmpty(method))
            {
                var parameterValidationResult = await _validationService.ValidateAsync(method);

                if (!parameterValidationResult.IsValid)
                    return parameterValidationResult.ToValidationProblemDetails();
            }

            CreateOrderDtoApp createOrderDtoApp = _mapper.Map<CreateOrderDtoApp>(createOrderRequest);

            EntityResult<OrderResponse> result = await _orderService.CreateOrderAsync(method, createOrderDtoApp);

            return result.IsSuccess ? Ok(result.Entity) : result.ToProblemDetails();
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            EntityResult<OrderResponse> result = await _orderService.GetByIdAsync(orderId);

            return result.IsSuccess ? Ok(result.Entity) : result.ToProblemDetails();
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            EntityResult<OrderResponse> result = await _orderService.DeleteOrderAsync(orderId);

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }

        [HttpPatch("{orderId}/mark-as-deleted")]
        public async Task<IActionResult> MarkOrderAsDeleted(int orderId)
        {
            EntityResult<OrderResponse> result = await _orderService.MarkOrderAsDeletedAsync(orderId);

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            EntityResult<IEnumerable<OrdersResDto>> result = await _orderService.GetOrdersAsync();

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }
    }
}