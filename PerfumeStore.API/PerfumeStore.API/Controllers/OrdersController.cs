using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PerfumeStore.API.Shared.DTO.Request.Order;
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

        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] CrateOrderDtoApi createOrderRequest)
        {
            var validationResult = await _validationService.ValidateAsync(createOrderRequest);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            CreateOrderDtoApp createOrderDtoApp = _mapper.Map<CreateOrderDtoApp>(createOrderRequest);

            EntityResult<OrderResponse> result = await _orderService.CreateOrderAsync(createOrderDtoApp);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetOrderById), new { orderId = result.Entity.Id }, result.Entity);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Wrong order id");

            EntityResult<OrderResponse> result = await _orderService.GetByIdAsync(orderId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Wrong order id");

            EntityResult<OrderResponse> result = await _orderService.DeleteOrderAsync(orderId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpPatch("{orderId}/mark-as-deleted")]
        public async Task<IActionResult> MarkOrderAsDeleted(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Wrong order id");

            EntityResult<OrderResponse> result = await _orderService.MarkOrderAsDeletedAsync(orderId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            EntityResult<IEnumerable<OrdersResDto>> result = await _orderService.GetOrdersAsync();

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }
    }
}