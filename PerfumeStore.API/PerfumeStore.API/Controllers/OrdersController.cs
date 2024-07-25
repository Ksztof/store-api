using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PerfumeStore.API.Shared.DTO.Request.Order;
using PerfumeStore.API.Shared.Extensions;
using PerfumeStore.API.Validators;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Orders;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.Entities.Products;

namespace PerfumeStore.API.Controllers
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
            
            if (!string.IsNullOrEmpty(method)){
                var parameterValidationResult = await _validationService.ValidateAsync(method);

                if (!parameterValidationResult.IsValid)
                    return parameterValidationResult.ToValidationProblemDetails();
            }

            CreateOrderDtoApp createOrderDtoApp = _mapper.Map<CreateOrderDtoApp>(createOrderRequest);

            EntityResult<OrderResponse> result = await _orderService.CreateOrderAsync(method, createOrderDtoApp);

            CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetOrderById), new { orderId = result.Entity?.Id }, result.Entity);

            return result.IsSuccess ? creationResult : result.ToProblemDetails();
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