using Store.Domain.Abstractions;

namespace Store.Domain.Carts;

public interface ICartsRepository
{
    public Task<Cart> CreateAsync(Cart item);
    public Task<Cart> UpdateAsync(Cart item);
    public Task<EntityResult<Cart>> GetByIdAsync(int cartId);
    public Task<EntityResult<Cart>> GetByUserIdAsync(string userId);
    public Task<Cart> GetByUserEmailAsync(string email);
    public Task DeleteAsync(Cart cart);
    public Task<Result<DateTime>> GetCartDateByIdAsync(int cartId);
    public Task<Result<int>> GetCartIdByUserIdAsync(string userId);
}