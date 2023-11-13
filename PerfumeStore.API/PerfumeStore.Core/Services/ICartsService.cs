using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.Core.Services
{
  public interface ICartsService
  {
    public Task<CartResponse> AddProductToCartAsync(int productId, decimal productQuantity);

    public Task<CartResponse> GetCartResponseByIdAsync(int cartId);
    public Task<Cart> GetCartByIdAsync(int cartId);

    public Task<CartResponse> DeleteCartLineFromCartAsync(int productId);

    public Task<CartResponse> SetProductQuantityAsync(int productId, decimal productQuantity);

    public Task<AboutCartResponse> CheckCartAsync();

    public Task<CartResponse> ClearCartAsync();
  }
}