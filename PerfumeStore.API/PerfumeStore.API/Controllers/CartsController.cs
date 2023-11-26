using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.DTOs.Request;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Core.DTO;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartsService _cartsService;
        private readonly IMapper _mapper;

        public CartsController(ICartsService cartsService, IMapper mapper)
        {
            _cartsService = cartsService;
            _mapper = mapper;
        }

        [HttpPost("products")]
        public async Task<IActionResult> AddProductToCartAsync([FromBody] AddProductsToCartRequest request)
        {
            AddProductsToCartDtoApplication modifyProductDto = _mapper.Map<AddProductsToCartDtoApplication>(request);

            Result<CartResponse> result = await _cartsService.AddProductsToCartAsync(modifyProductDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction("GetCartById", new { cartId = result.Entity.Id}, result.Entity);
        }

        [HttpPut("products/{productId}")]
        public async Task<IActionResult> DeleteProductLineFromCartAsync(int productId)
        {
            Result<CartResponse> result = await _cartsService.DeleteCartLineFromCartAsync(productId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpPut("products")]
        public async Task<IActionResult> ModifyProductAsync([FromBody] ModifyProductRequest modifiedProduct)
        {
            ModifyProductDtoApplication modifyProductDto = _mapper.Map<ModifyProductDtoApplication>(modifiedProduct);

            Result<CartResponse> result = await _cartsService.ModifyProductAsync(modifyProductDto);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpGet]
        public async Task<IActionResult> CheckCartAsync()
        {
            Result<AboutCartRes> products = await _cartsService.CheckCartAsync();

            return Ok(products);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCartAsync()
        {
            Result<CartResponse> result = await _cartsService.ClearCartAsync();
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpGet("{cartId}", Name = "GetCartById")]
        public async Task<IActionResult> GetCartByIdAsync(int cartId)
        {
            Result<CartResponse> result = await _cartsService.GetCartResponseByIdAsync(cartId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }
    }


}