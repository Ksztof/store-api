namespace PerfumeStore.Core.CustomExceptions
{
    public class EmptyProductListException : Exception
    {
        public EmptyProductListException(string message)
            : base(message) { }
        public EmptyProductListException(string message, Exception emptyProductListException)
            : base(message, emptyProductListException) { }
    }
}
