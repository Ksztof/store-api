namespace Store.Domain.ProductCategories;

public interface IProductCategoriesRepository
{
    public Task<ProductCategory?> GetByIdAsync(int id);
    public Task<ICollection<ProductCategory>> GetByIdsAsync(ICollection<int> ids);
}