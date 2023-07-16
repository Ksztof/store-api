using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.RequestForms;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsService(IProductsRepository productsRepository, IProductCategoriesRepository productCategoriesRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<Product> CreateProductAsync(CreateProductForm createProductForm)
        {
            var productToCreate = new Product
            {
                Name = createProductForm.ProductName,
                Price = createProductForm.ProductPrice,
                Description = createProductForm.ProductDescription,
                CategoryId = createProductForm.ProductCategoryId,
                Manufacturer = createProductForm.ProductManufacturer,
                DateAdded = DateTime.Now
            };

            Product createdProductId = await _productsRepository.CreateAsync(productToCreate);
            return createdProductId;
        }

        public async Task DeleteProductAsync(int productId)
        {
            Product product = await _productsRepository.GetByIdAsync(productId);
            await _productsRepository.DeleteAsync(product);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            IEnumerable<Product> productsList = await _productsRepository.GetAllAsync();
            return productsList;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            Product product = await _productsRepository.GetByIdAsync(productId);
            return await Task.FromResult(product);
        }

        public async Task<Product> UpdateProductAsync(UpdateProductForm updateform)
        {
            Product productToUpdate = await _productsRepository.GetByIdAsync(updateform.ProductId); //TODO:will have to think about smarter mapping/mapper
            productToUpdate.Name = updateform.ProductName;
            productToUpdate.Price = updateform.ProductPrice;
            productToUpdate.Description = updateform.ProductDescription;
            productToUpdate.Manufacturer = updateform.ProductManufacturer;

            Product updatedProductId = await _productsRepository.UpdateAsync(productToUpdate);

            return await Task.FromResult(updatedProductId);
        }
    }
}
