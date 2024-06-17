using AutoMapper;
using PerfumeStore.Application.Abstractions;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Contracts.JwtToken;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.DTO.Request.Product;
using PerfumeStore.Domain.Entities.ProductCategories;
using PerfumeStore.Domain.Entities.Products;
using PerfumeStore.Domain.Repositories;

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
            Product? productExists = await _productsRepository.GetByName(createProductForm.ProductName);

            if (productExists != null)
            {
                Error error = EntityErrors<Product, int>.ProductAlreadyExists(productExists.Id, createProductForm.ProductName);

                return EntityResult<ProductResponse>.Failure(error);
            }

            ICollection<ProductCategory> productCategories = await _productCategoriesRepository.GetByIdsAsync(createProductForm.ProductCategoriesIds);

            if (productCategories.Count != createProductForm.ProductCategoriesIds.Count)
            {
                int[] foundIds = productCategories.Select(pc => pc.Id).ToArray();
                int[] notFoundIds = createProductForm.ProductCategoriesIds.Except(foundIds).ToArray();

                Error error = EntityErrors<ProductCategory, int>.NotFoundEntitiesByIds(notFoundIds);

                return EntityResult<ProductResponse>.Failure(error);
            }

            CreateProductDtoDom createProductDtoDom = _mapper.Map<CreateProductDtoDom>(createProductForm);

            Product product = new Product();
            product.CreateProduct(createProductDtoDom, productCategories);
            product = await _productsRepository.CreateAsync(product);
            ProductResponse productDetails = MapProductResponse(product);

            return EntityResult<ProductResponse>.Success(productDetails);
        }

        public async Task<EntityResult<Product>> DeleteProductAsync(int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);

            if (product == null)
            {
                var error = EntityErrors<Product, int>.NotFound(productId);

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
                var productError = EntityErrors<Product, int>.NotFound(productId);
                return EntityResult<ProductResponse>.Failure(productError);
            }

            ProductResponse productDetails = MapProductResponse(product);

            return EntityResult<ProductResponse>.Success(productDetails);
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            IEnumerable<Product> products = await _productsRepository.GetAllAsync();

            IEnumerable<ProductResponse> productsDetails = MapProductsToResponse(products);

            return productsDetails;
        }

        public async Task<EntityResult<ProductResponse>> UpdateProductAsync(UpdateProductDtoApp updateForm)
        {
            Product? product = await _productsRepository.GetByIdAsync(updateForm.productId);

            if (product is null)
            {
                var error = EntityErrors<Product, int>.NotFound(updateForm.productId);

                return EntityResult<ProductResponse>.Failure(error);
            }

            ICollection<ProductCategory> newProductCategories = await _productCategoriesRepository.GetByIdsAsync(updateForm.ProductCategoriesIds);

            if (newProductCategories.Count != updateForm.ProductCategoriesIds.Count)
            {
                var foundIds = newProductCategories.Select(pc => pc.Id);
                var notFoundIds = updateForm.ProductCategoriesIds.Except(foundIds);
                var error = EntityErrors<Product, int>.NotFoundEntitiesByIds(notFoundIds);

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