namespace PerfumeStore.Core.CustomExceptions
{
  public class MissingDataInCookieException : Exception
  {
    public MissingDataInCookieException(string message) : base(message)
    {
    }

    public MissingDataInCookieException(string message, Exception ex) : base(message, ex)
    {
    }
  }
}