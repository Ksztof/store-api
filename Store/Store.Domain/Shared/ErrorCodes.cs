namespace Store.Domain.Shared
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Authentication = 4,
        Authorization = 5,
        Server = 6,
    }
}
