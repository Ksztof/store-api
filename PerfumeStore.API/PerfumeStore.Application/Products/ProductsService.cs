using Castle.Core.Internal;
using PerfumeStore.Application.CustomExceptions;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.Products;
using PerfumeStore.Domain.Results;
using PerfumeStore.Domain.Tokens;

namespace PerfumeStore.Application.Products
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IProductCategoriesRepository _productCategoriesRepository;
        private readonly ITokenService _tokenService;

        public ProductsService(IProductsRepository productsRepository, IProductCategoriesRepository productCategoriesRepository, ITokenService tokenService)
        {
            _productsRepository = productsRepository;
            _productCategoriesRepository = productCategoriesRepository;
            _tokenService = tokenService;
        }

        public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductForm createProductForm)
        {
            ICollection<ProductCategory> productCategories = await _productCategoriesRepository.GetByIdsAsync(createProductForm.ProductCategoriesIds);

            if (productCategories.Count != createProductForm.ProductCategoriesIds.Count)
            {
                IEnumerable<int> foundIds = productCategories.Select(pc => pc.Id);
                IEnumerable<int> notFoundIds = createProductForm.ProductCategoriesIds.Except(foundIds);

                return Result<ProductResponse>.Failure(EntityErrors<ProductCategory, int>.MissingEntities(notFoundIds));
            }

            Product product = new Product();
            product.CreateProduct(createProductForm, productCategories);
            product = await _productsRepository.CreateAsync(product);
            ProductResponse productResponse = MapProductResponse(product);

            return Result<ProductResponse>.Success(productResponse);
        }

        public async Task<Result<Product>> DeleteProductAsync(int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return Result<Product>.Failure(EntityErrors<ProductCategory, int>.MissingEntity(productId));
            }

            await _productsRepository.DeleteAsync(productId);

            return Result<Product>.Success(product);
        }
        
        public async Task<Result<ProductResponse>> GetProductByIdAsync(int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product is null)
            {
                return Result<ProductResponse>.Failure(EntityErrors<Product, int>.MissingEntity(productId));
            }

            ProductResponse productResponse = MapProductResponse(product);

            return Result<ProductResponse>.Success(productResponse);
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            IEnumerable<Product> products = await _productsRepository.GetAllAsync();
            if (products.IsNullOrEmpty())
            {
                IEnumerable<ProductResponse> emptyCollection = new List<ProductResponse>();

                return emptyCollection;
            }

            IEnumerable<ProductResponse> productsResponse = MapProductsToResponse(products);

            return productsResponse;
        }

        public async Task<Result<ProductResponse>> UpdateProductAsync(UpdateProductForm updateForm, int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product is null)
            {
                return Result<ProductResponse>.Failure(EntityErrors<Product, int>.MissingEntity(productId));
            }

            ICollection<ProductCategory> newProductCategories = await _productCategoriesRepository.GetByIdsAsync(updateForm.ProductCategoriesIds);
            if (newProductCategories.Count != updateForm.ProductCategoriesIds.Count)
            {
                var foundIds = newProductCategories.Select(pc => pc.Id);
                var notFoundIds = updateForm.ProductCategoriesIds.Except(foundIds);

                return Result<ProductResponse>.Failure(EntityErrors<Product, int>.MissingEntities(notFoundIds));
            }

            product.UpdateProduct(updateForm, newProductCategories);
            product = await _productsRepository.UpdateAsync(product);
            ProductResponse productResponse = MapProductResponse(product);

            return Result<ProductResponse>.Success(productResponse);
        }

        private static IEnumerable<ProductResponse> MapProductsToResponse(IEnumerable<Product> products)
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

        private static ProductResponse MapProductResponse(Product? product)
        {
            ProductResponse productResponse = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Manufacturer = product.Manufacturer,
                DateAdded = product.DateAdded,
            };

            return productResponse;
        }
    }
}