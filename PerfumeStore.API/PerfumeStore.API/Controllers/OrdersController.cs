using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.Orders;
using PerfumeStore.Domain.Abstractions;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public readonly IOrdersService _orderService;

        public OrdersController(IOrdersService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder()
        {
            EntityResult<OrderResponse> result = await _orderService.CreateOrderAsync();

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
    }
}