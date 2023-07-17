namespace PerfumeStore.Core.CustomExceptions
{
    public class ProductCategoryNotFoundEx : Exception
    {
        public ProductCategoryNotFoundEx(string message)
            : base(message) { }
        public ProductCategoryNotFoundEx(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
