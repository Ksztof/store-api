using PerfumeStore.Core.CustomExceptions;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Repositories;
using PerfumeStore.Domain.DbModels;
using PerfumeStore.Domain.Models;

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

        public async Task<ProductResponse> CreateProductAsync(CreateProductForm createProductForm)
        {
            ICollection<ProductCategory> productCategories = await _productCategoriesRepository.GetByIdsAsync(createProductForm.ProductCategoriesIds);
            if (productCategories.Count != createProductForm.ProductCategoriesIds.Count)
            {
                var foundIds = productCategories.Select(pc => pc.Id);
                var notFoundIds = createProductForm.ProductCategoriesIds.Except(foundIds);
                string notFoundIdsString = string.Join(", ", notFoundIds);

                throw new EntityNotFoundException<ProductCategory, int>($"Can't find entities {typeof(ProductCategory)} with Ids: {notFoundIdsString}");
            }

            Product product = new Product();
            product.CreateProduct(createProductForm, productCategories);
            product = await _productsRepository.CreateAsync(product);
            ProductResponse productDto = MapProductDto(product);


            return productDto;
        }

        public async Task DeleteProductAsync(int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new EntityNotFoundException<Product, int>($"Entity of type: {typeof(Product)} is missing. Entity Id: {productId}");
            }

            await _productsRepository.DeleteAsync(productId);
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            IEnumerable<Product> products = await _productsRepository.GetAllAsync();
            IEnumerable<ProductResponse> productsDto = MapProductsToDto(products);
            return productsDto;
        }

        public async Task<ProductResponse> GetProductByIdAsync(int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product is null)
            {
                throw new EntityNotFoundException<Product, int>($"Can't find product with given id. Product:  {product}");
            }

            ProductResponse productDto = MapProductDto(product);

            return productDto;
        }

        public async Task<ProductResponse> UpdateProductAsync(UpdateProductForm updateForm)
        {
            Product? product = await _productsRepository.GetByIdAsync(updateForm.ProductId);
            if (product is null)
            {
                throw new EntityNotFoundException<Product, int>($"Can't find entity {typeof(Product)} with Id: {updateForm.ProductId}");
            }

            ICollection<ProductCategory> newProductCategories = await _productCategoriesRepository.GetByIdsAsync(updateForm.ProductCategoriesIds);
            if (newProductCategories.Count != updateForm.ProductCategoriesIds.Count)
            {
                var foundIds = newProductCategories.Select(pc => pc.Id);
                var notFoundIds = updateForm.ProductCategoriesIds.Except(foundIds);
                string notFoundIdsString = string.Join(", ", notFoundIds);

                throw new EntityNotFoundException<ProductCategory, int>($"Can't find entities {typeof(ProductCategory)} with Ids: {notFoundIdsString}");
            }

            product.UpdateProduct(updateForm, newProductCategories);
            product = await _productsRepository.UpdateAsync(product);
            ProductResponse productDto = MapProductDto(product);


            return productDto;
        }
        private static IEnumerable<ProductResponse> MapProductsToDto(IEnumerable<Product> products)
        {
            return products.Select(x => new ProductResponse
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Description = x.Description,
                Manufacturer = x.Manufacturer,
                DateAdded = x.DateAdded,
            });
        }
        private static ProductResponse MapProductDto(Product? product)
        {
            ProductResponse productDto = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Manufacturer = product.Manufacturer,
                DateAdded = product.DateAdded,
            };

            return productDto;
        }
    }
}

