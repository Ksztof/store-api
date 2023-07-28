﻿using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Services;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productService;

        public ProductsController(IProductsService productService)
        {
            _productService = productService;
        }

        [HttpPost("CreateProductAsync")]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductForm createProductForm)
        {
            ProductDto createdProduct = await _productService.CreateProductAsync(createProductForm);
            return Ok(createdProduct);
        }

        [HttpPut("UpdateProductAsync")]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductForm updateform)
        {
            ProductDto updatedProductId = await _productService.UpdateProductAsync(updateform);
            return Ok(updatedProductId);
        }

        [HttpDelete("DeleteProductAsync/{productId}")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            await _productService.DeleteProductAsync(productId);
            return NoContent();
        }

        [HttpGet("GetProductByIdAsync/{productId}")]
        public async Task<IActionResult> GetProductByIdAsync(int productId)
        {
            ProductDto product = await _productService.GetProductByIdAsync(productId);
            return Ok(product);
        }

        [HttpGet("GetAllProductsAsync")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            IEnumerable<ProductDto> products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
    }
}
