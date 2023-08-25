using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Services;

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
            OrderResponse order = await _orderService.CreateOrderAsync();

            return CreatedAtAction("GetOrderById", new { orderId = order.Id }, order);
        }

        [HttpGet("{orderId}", Name = "GetOrderById")]
        public async Task<IActionResult> GetOrderByIdAsync(int orderId)
        {
            OrderResponse order = await _orderService.GetByIdAsync(orderId);

            return Ok(order);
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrderAsync(int orderId)
        {
            await _orderService.DeleteOrderAsync(orderId);

            return NoContent();
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> MarkOrderAsDeletedAsync(int orderId)
        {
            await _orderService.MarkOrderAsDeletedAsync(orderId);

            return NoContent();
        }
    }
}