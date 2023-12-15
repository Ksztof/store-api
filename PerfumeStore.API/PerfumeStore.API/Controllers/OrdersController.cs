using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.DTOs.Request;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.Orders;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Orders;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public readonly IOrdersService _orderService;
        private readonly IMapper _mapper;

        public OrdersController(IOrdersService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder([FromBody] CrateOrderDtoApi createOrderRequest)
        {
            CreateOrderDtoApp createOrderDtoApp = _mapper.Map<CreateOrderDtoApp>(createOrderRequest);         

            EntityResult<OrderResponse> result = await _orderService.CreateOrderAsync(createOrderDtoApp);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction("GetOrderById", new { orderId = result.Entity.Id }, result.Entity);
        }

        [HttpGet("{orderId}", Name = "GetOrderById")]
        public async Task<IActionResult> GetOrderByIdAsync(int orderId)
        {
            EntityResult<OrderResponse> result = await _orderService.GetByIdAsync(orderId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrderAsync(int orderId)
        {
            EntityResult<OrderResponse> result = await _orderService.DeleteOrderAsync(orderId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> MarkOrderAsDeletedAsync(int orderId)
        {
            EntityResult<OrderResponse> result = await _orderService.MarkOrderAsDeletedAsync(orderId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync()
        {
            IEnumerable<Order> result = await _orderService.GetOrdersAsync();

           

            return Ok(result);
        }
    }
}