namespace PerfumeStore.Core.CustomExceptions
{
    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException()
        {
        }

        public ProductNotFoundException(string message)
            : base(message)
        {
        }

        public ProductNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
