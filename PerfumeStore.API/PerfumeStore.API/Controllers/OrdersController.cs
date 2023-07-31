using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Services;
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

        [HttpGet]
        public async Task<IActionResult> SubmitOrder()
        {
            AboutCartResponse order = await _orderService.CreateOrderAsync();
            return Ok();
        }
    }
}
