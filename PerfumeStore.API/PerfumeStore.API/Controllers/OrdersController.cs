using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Services;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;

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

        [HttpPost]//?? Is Get ok here
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
            _orderService.DeleteOrderAsync(orderId);

            return Ok();
        }

        [HttpPut("{orderId}/cancel")]
        public async Task<IActionResult> MarkOrderAsDeletedAsync(int orderId)
        {
            _orderService.MarkOrderAsDeletedAsync(orderId);
            
            return Ok();
        }
    }
}
