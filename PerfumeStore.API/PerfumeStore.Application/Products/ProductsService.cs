using AutoMapper;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Errors;
using PerfumeStore.Domain.ProductCategories;
using PerfumeStore.Domain.Products;
using PerfumeStore.Domain.Tokens;

namespace PerfumeStore.Application.Products
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IProductCategoriesRepository _productCategoriesRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public ProductsService(
            IProductsRepository productsRepository,
            IProductCategoriesRepository productCategoriesRepository,
            ITokenService tokenService,
            IMapper mapper)
        {
            _productsRepository = productsRepository;
            _productCategoriesRepository = productCategoriesRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<EntityResult<ProductResponse>> CreateProductAsync(CreateProductDtoApp createProductForm)
        {
            ICollection<ProductCategory> productCategories = await _productCategoriesRepository.GetByIdsAsync(createProductForm.ProductCategoriesIds);

            if (productCategories.Count != createProductForm.ProductCategoriesIds.Count)
            {
                int[] foundIds = productCategories.Select(pc => pc.Id).ToArray();
                int[] notFoundIds = createProductForm.ProductCategoriesIds.Except(foundIds).ToArray();

                Error error = EntityErrors<ProductCategory, int>.MissingEntities(notFoundIds);

                return EntityResult<ProductResponse>.Failure(error);
            }

            CreateProductDtoDom createProductDtoDom = _mapper.Map<CreateProductDtoDom>(createProductForm);

            Product product = new Product();
            product.CreateProduct(createProductDtoDom, productCategories);
            product = await _productsRepository.CreateAsync(product);
            ProductResponse productResponse = MapProductResponse(product);

            return EntityResult<ProductResponse>.Success(productResponse);
        }

        public async Task<EntityResult<Product>> DeleteProductAsync(int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product == null)
            {
                var error = EntityErrors<Product, int>.MissingEntity(productId);

                return EntityResult<Product>.Failure(error);
            }

            await _productsRepository.DeleteAsync(productId);

            return EntityResult<Product>.Success();
        }

        public async Task<EntityResult<ProductResponse>> GetProductByIdAsync(int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product is null)
            {
                var productError = EntityErrors<Product, int>.MissingEntity(productId);
                return EntityResult<ProductResponse>.Failure(productError);
            }

            ProductResponse productResponse = MapProductResponse(product);

            return EntityResult<ProductResponse>.Success(productResponse);
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            IEnumerable<Product> products = await _productsRepository.GetAllAsync();

            IEnumerable<ProductResponse> productsResponse = MapProductsToResponse(products);

            return productsResponse;
        }

        public async Task<EntityResult<ProductResponse>> UpdateProductAsync(UpdateProductDtoApp updateForm)
        {
            Product? product = await _productsRepository.GetByIdAsync(updateForm.productId);
            if (product is null)
            {
                var error = EntityErrors<Product, int>.MissingEntity(updateForm.productId);

                return EntityResult<ProductResponse>.Failure(error);
            }

            ICollection<ProductCategory> newProductCategories = await _productCategoriesRepository.GetByIdsAsync(updateForm.ProductCategoriesIds);
            if (newProductCategories.Count != updateForm.ProductCategoriesIds.Count)
            {
                var foundIds = newProductCategories.Select(pc => pc.Id);
                var notFoundIds = updateForm.ProductCategoriesIds.Except(foundIds);

                var error = EntityErrors<Product, int>.MissingEntities(notFoundIds);

                return EntityResult<ProductResponse>.Failure(error);
            }

            UpdateProductDtoDom updateProductDtoDom = _mapper.Map<UpdateProductDtoDom>(updateForm);

            product.UpdateProduct(updateProductDtoDom, newProductCategories);
            product = await _productsRepository.UpdateAsync(product);
            ProductResponse productResponse = MapProductResponse(product);

            return EntityResult<ProductResponse>.Success(productResponse);
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