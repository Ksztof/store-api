namespace PerfumeStore.Application.CustomExceptions
{
    public class MissingDataInCookieEx : Exception
    {
        public MissingDataInCookieEx(string message) : base(message)
        {
        }

        public MissingDataInCookieEx(string message, Exception ex) : base(message, ex)
        {
        }
    }
}