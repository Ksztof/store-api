using IdentityModel.Client;
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
    private readonly ITokenService _tokenService;
    private readonly IValidationService _validationService;
    public ProductsService(IProductsRepository productsRepository, IProductCategoriesRepository productCategoriesRepository, ITokenService tokenService, IValidationService validationService)
    {
      _productsRepository = productsRepository;
      _productCategoriesRepository = productCategoriesRepository;
      _tokenService = tokenService;
      _validationService = validationService;
    }

    public async Task<ProductResponse> CreateProductAsync(CreateProductForm createProductForm)
    {
      var formValidation = _validationService.ValidateCreateProductForm(createProductForm);
      if (!formValidation.IsValid)
      {
        IEnumerable<string> validationErrors = formValidation.Errors.Select(x => x.ErrorMessage).ToList();

        throw new Exception($"Validation errors: {validationErrors}");
      }

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
      ProductResponse productResponse = MapProductResponse(product);

      return productResponse;
    }

    public async Task DeleteProductAsync(int productId)
    {
      var idValidation = _validationService.ValidateEntityId(productId);
      if (!idValidation.IsValid)
      {
        IEnumerable<string> validationErrors = idValidation.Errors.Select(x => x.ErrorMessage).ToList();

        throw new Exception($"Validation errors: {validationErrors}");
      }

      Product? product = await _productsRepository.GetByIdAsync(productId);
      if (product == null)
      {
        throw new EntityNotFoundException<Product, int>($"Entity of type: {typeof(Product)} is missing. Entity Id: {productId}");
      }

      await _productsRepository.DeleteAsync(productId);
    }

    public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
    {
      using var client = new HttpClient();

      var token = await _tokenService.GetToken("PerfumeStore.read");

      client.SetBearerToken(token.AccessToken);

      //var result = await client.GetAsync("https://localhost:5445/api/Products");

      IEnumerable<Product> products = await _productsRepository.GetAllAsync();
      IEnumerable<ProductResponse> productsResponse = MapProductsToResponse(products);

      return productsResponse;
    }

    public async Task<ProductResponse> GetProductByIdAsync(int productId)
    {
      var idValidation = _validationService.ValidateEntityId(productId);
      if (!idValidation.IsValid)
      {
        IEnumerable<string> validationErrors = idValidation.Errors.Select(x => x.ErrorMessage).ToList();

        throw new Exception($"Validation errors: {validationErrors}");
      }

      Product? product = await _productsRepository.GetByIdAsync(productId);
      if (product is null)
      {
        throw new EntityNotFoundException<Product, int>($"Can't find product with given id. Product:  {product}");
      }

      ProductResponse productResponse = MapProductResponse(product);

      return productResponse;
    }

    public async Task<ProductResponse> UpdateProductAsync(UpdateProductForm updateForm, int productId)
    {
      var formValidation = _validationService.ValidateUpdateProductForm(updateForm);
      var idValidation = _validationService.ValidateEntityId(productId);

      if (!formValidation.IsValid && !idValidation.IsValid)
      {
        IEnumerable<string> formErrors = formValidation.Errors.Select(x => x.ErrorMessage).ToList();
        IEnumerable<string> idErrors = idValidation.Errors.Select(x => x.ErrorMessage).ToList();

        throw new Exception($"Validation errors: Update form errors: {formErrors} Product Id errors: {idErrors}");
      }

      Product? product = await _productsRepository.GetByIdAsync(productId);
      if (product is null)
      {
        throw new EntityNotFoundException<Product, int>($"Can't find entity {typeof(Product)} with Id: {productId}");
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
      ProductResponse productResponse = MapProductResponse(product);

      return productResponse;
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