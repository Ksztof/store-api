namespace PerfumeStore.Core.CustomExceptions
{
    public class CantDeleteProductEx : Exception
    {
        public CantDeleteProductEx(string message)
            : base(message) { }
        public CantDeleteProductEx(string message, Exception cantDeleteProductEx)
           : base(message, cantDeleteProductEx) { }
    }
}
