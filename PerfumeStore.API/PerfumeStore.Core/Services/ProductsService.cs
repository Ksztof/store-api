using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Core.RequestForms;
using PerfumeStore.Domain.DbModels;

namespace PerfumeStore.Core.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IProductCategoriesRepository _productCategoriesRepository;

        public ProductsService(IProductsRepository productsRepository, IProductCategoriesRepository productCategoriesRepository)
        {
            _productsRepository = productsRepository;
            _productCategoriesRepository = productCategoriesRepository;
        }

        public async Task<Product> CreateProductAsync(CreateProductForm createProductForm)
        {
            ICollection<ProductCategory> productCategories = await _productCategoriesRepository.GetByIdsAsync(createProductForm.ProductCategoryId);
            if (!productCategories.Any())
            {
                throw new EntityNotFoundException<ProductCategory, int>("Product categories are missing.");
            }
            Product newProduct = GenerateNewProduct(createProductForm, productCategories);
            newProduct = await _productsRepository.CreateAsync(newProduct);

            return newProduct;
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _productsRepository.DeleteAsync(productId);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            IEnumerable<Product> productsList = await _productsRepository.GetAllAsync();
            return productsList;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {

            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product is null)
            {
                throw new EntityNotFoundException<Product, int>($"Can't find product with given id. Product:  {product}");
            }
            return product;
        }

        public async Task<Product> UpdateProductAsync(UpdateProductForm updateForm)
        {
            Product? productToUpdate = await _productsRepository.GetByIdAsync(updateForm.ProductId);
            if (productToUpdate is null)
            {
                throw new EntityNotFoundException<Product, int>($"Can't find entity {typeof(Product)} with Id: {updateForm.ProductId}");
            }
            Product updatedProductId = await _productsRepository.UpdateAsync(productToUpdate);

            return updatedProductId;
        }

        private Product GenerateNewProduct(CreateProductForm createProductForm, ICollection<ProductCategory> productCategories)
        {
            var productToCreate = new Product
            {
                Name = createProductForm.ProductName,
                Price = createProductForm.ProductPrice,
                Description = createProductForm.ProductDescription,
                ProductCategories = productCategories,
                Manufacturer = createProductForm.ProductManufacturer,
                DateAdded = DateTime.Now
            };

            return productToCreate;
        }

        private static void GenerateUpdatedProduct(UpdateProductForm updateform, Product productToUpdate)
        {
            productToUpdate.Name = updateform.ProductName;
            productToUpdate.Price = updateform.ProductPrice;
            productToUpdate.Description = updateform.ProductDescription;
            productToUpdate.Manufacturer = updateform.ProductManufacturer;
        }
    }
}

