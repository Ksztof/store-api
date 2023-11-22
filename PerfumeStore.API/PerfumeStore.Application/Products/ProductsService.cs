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

        public async Task<Result<Product>> CreateProductAsync(CreateProductForm createProductForm)
        {
            ICollection<ProductCategory> productCategories = await _productCategoriesRepository.GetByIdsAsync(createProductForm.ProductCategoriesIds);

            if (productCategories.Count != createProductForm.ProductCategoriesIds.Count)
            {
                IEnumerable<int> foundIds = productCategories.Select(pc => pc.Id);
                IEnumerable<int> notFoundIds = createProductForm.ProductCategoriesIds.Except(foundIds);
                string notFoundIdsString = string.Join(", ", notFoundIds);

                return Result<Product>.Failure(EntityErrors<ProductCategory, int>.MissingEntities(notFoundIds));
            }

            Product product = new Product();
            product.CreateProduct(createProductForm, productCategories);
            product = await _productsRepository.CreateAsync(product);
            ProductResponse productResponse = MapProductResponse(product);

            return Result<Product>.Success(product);
        }

        public async Task<Result<Product>> DeleteProductAsync(int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return Result<Product>.Success(product);
            }

            await _productsRepository.DeleteAsync(productId);

            return Result<Product>.Success(product);
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            /*Problem jest taki że deklaruję typ zwracany: public async Task<Result>
             no i w przypadku errora, wszystko działa cacy, mogę generycznie robić errory dla różnych typów danych, no a jak się uda wszystko, to mogę zwrócić: return Result.Success(); - No i teraz, chciałbym przy zwracaniu sukcesu mieć możliwość przekazania obiektu. No i oczywiście żeby nie pisać 100 różnych konstruktorów na każdy typ chciałbym to obsłużyć jakoś generycznie, ale jak dam w Result typ generyczny np. Result<Product>, to nie zwrócę błędu z Cart, albo CartLine, bo to że jestem w serwisie product to nie znaczy że wyszstko będę zwracał z produktem, np. tworzenie produktu może się nie udać, bo ktoś podał id kategorii z dupy i wtedy muszę zwrócić `return EntityErrors<ProductCategory, int>.MissingEntities(notFoundIds);` 
              - Więc opcje które ja widzę to stworzyć coś na kształt klasy Error i EntityErros dla success i rzutować to implictem do Resulta jak w.w 
              - druga opcja to wykorzystać jakoś `object` ale to lipton chyba.
 */
            IEnumerable<Product> products = await _productsRepository.GetAllAsync();
            IEnumerable<ProductResponse> productsResponse = MapProductsToResponse(products);

            return productsResponse;
        }

        public async Task<ProductResponse> GetProductByIdAsync(int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product is null)
            {
                throw new EntityNotFoundEx<Product, int>($"Can't find product with given id. Product:  {product}");
            }

            ProductResponse productResponse = MapProductResponse(product);

            return productResponse;
        }

        public async Task<ProductResponse> UpdateProductAsync(UpdateProductForm updateForm, int productId)
        {
            Product? product = await _productsRepository.GetByIdAsync(productId);
            if (product is null)
            {
                throw new EntityNotFoundEx<Product, int>(product.Id);
            }

            ICollection<ProductCategory> newProductCategories = await _productCategoriesRepository.GetByIdsAsync(updateForm.ProductCategoriesIds);
            if (newProductCategories.Count != updateForm.ProductCategoriesIds.Count)
            {
                var foundIds = newProductCategories.Select(pc => pc.Id);
                var notFoundIds = updateForm.ProductCategoriesIds.Except(foundIds);
                string notFoundIdsString = string.Join(", ", notFoundIds);

                throw new EntityNotFoundEx<ProductCategory, int>(notFoundIdsString);
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