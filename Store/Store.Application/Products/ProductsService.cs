using AutoMapper;
using Store.Application.Products.Dto.Request;
using Store.Application.Products.Dto.Response;
using Store.Domain.Abstractions;
using Store.Domain.ProductCategories;
using Store.Domain.Products;
using Store.Domain.Products.Dto.Request;
using Store.Domain.Shared.Errors;

namespace Store.Application.Products;

public class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;
    private readonly IProductCategoriesRepository _productCategoriesRepository;
    private readonly IMapper _mapper;

    public ProductsService(
        IProductsRepository productsRepository,
        IProductCategoriesRepository productCategoriesRepository,
        IMapper mapper)
    {
        _productsRepository = productsRepository;
        _productCategoriesRepository = productCategoriesRepository;
        _mapper = mapper;
    }

    public async Task<EntityResult<ProductResponseDto>> CreateProductAsync(CreateProductDtoApp createProductForm)
    {
        EntityResult<Product> getProduct = await _productsRepository.GetByName(createProductForm.ProductName);

        if (getProduct.IsSuccess)
        {
            Error error = EntityErrors<Product, int>.ProductAlreadyExists(getProduct.Entity.Id, createProductForm.ProductName);
            return EntityResult<ProductResponseDto>.Failure(error);
        }

        ICollection<ProductCategory> productCategories = await _productCategoriesRepository.GetByIdsAsync(createProductForm.ProductCategoriesIds);

        if (productCategories.Count != createProductForm.ProductCategoriesIds.Count)
        {
            int[] foundIds = productCategories.Select(pc => pc.Id).ToArray();
            int[] notFoundIds = createProductForm.ProductCategoriesIds.Except(foundIds).ToArray();

            Error error = EntityErrors<ProductCategory, int>.NotFoundEntitiesByIds(notFoundIds);
            return EntityResult<ProductResponseDto>.Failure(error);
        }

        CreateProductDtoDom createProductDtoDom = _mapper.Map<CreateProductDtoDom>(createProductForm);

        Product product = new Product();

        product.CreateProduct(createProductDtoDom, productCategories);
        product = await _productsRepository.CreateAsync(product);

        ProductResponseDto productDetails = MapProductResponse(product);

        return EntityResult<ProductResponseDto>.Success(productDetails);
    }

    public async Task<EntityResult<Product>> DeleteProductAsync(int productId)
    {
        if (productId <= 0)
        {
            return EntityResult<Product>.Failure(EntityErrors<Product, int>.WrongEntityId(productId));
        }

        EntityResult<Product> getProduct = await _productsRepository.GetByIdAsync(productId);

        if (getProduct.IsFailure)
        {
            return EntityResult<Product>.Failure(getProduct.Error);
        }

        await _productsRepository.DeleteAsync(productId);

        return EntityResult<Product>.Success();
    }

    public async Task<EntityResult<ProductResponseDto>> GetProductByIdAsync(int productId)
    {
        if (productId <= 0)
        {
            return EntityResult<ProductResponseDto>.Failure(EntityErrors<Product, int>.WrongEntityId(productId));
        }

        EntityResult<Product> getProduct = await _productsRepository.GetByIdAsync(productId);

        if (getProduct.IsFailure)
        {
            return EntityResult<ProductResponseDto>.Failure(getProduct.Error);
        }

        ProductResponseDto productDetails = MapProductResponse(getProduct.Entity);

        return EntityResult<ProductResponseDto>.Success(productDetails);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        IEnumerable<Product> products = await _productsRepository.GetAllAsync();

        IEnumerable<ProductResponseDto> productsDetails = MapProductsToResponse(products);

        return productsDetails;
    }

    public async Task<EntityResult<ProductResponseDto>> UpdateProductAsync(UpdateProductDtoApp updateForm)
    {
        EntityResult<Product> getProduct = await _productsRepository.GetByIdAsync(updateForm.productId);

        if (getProduct.IsFailure)
        {
            return EntityResult<ProductResponseDto>.Failure(getProduct.Error);
        }

        Product product = getProduct.Entity;

        ICollection<ProductCategory> newProductCategories = await _productCategoriesRepository.GetByIdsAsync(updateForm.ProductCategoriesIds);

        if (newProductCategories.Count != updateForm.ProductCategoriesIds.Count)
        {
            IEnumerable<int> foundIds = newProductCategories.Select(pc => pc.Id);
            IEnumerable<int> notFoundIds = updateForm.ProductCategoriesIds.Except(foundIds);

            Error error = EntityErrors<Product, int>.NotFoundEntitiesByIds(notFoundIds);

            return EntityResult<ProductResponseDto>.Failure(error);
        }

        UpdateProductDtoDom updateProductDtoDom = _mapper.Map<UpdateProductDtoDom>(updateForm);

        product.UpdateProduct(updateProductDtoDom, newProductCategories);
        product = await _productsRepository.UpdateAsync(product);

        ProductResponseDto productResponse = MapProductResponse(product);

        return EntityResult<ProductResponseDto>.Success(productResponse);
    }

    private static IEnumerable<ProductResponseDto> MapProductsToResponse(IEnumerable<Product> products)
    {
        return products.Select(x => new ProductResponseDto
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            Description = x.Description,
            Manufacturer = x.Manufacturer,
            DateAdded = x.DateAdded,
        });
    }

    private static ProductResponseDto MapProductResponse(Product? product)
    {
        ProductResponseDto productResponse = new ProductResponseDto
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