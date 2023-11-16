namespace PerfumeStore.Domain.CustomExceptions
{
    public class UserModificationException : Exception
    {
        public string Operation { get; }
        public string userId { get; }

        public UserModificationException(string operation, string userId) : base($"User modification failed at operation {operation}, {userId}")
        {
            Operation = operation;
        }

        public UserModificationException(string operation, Exception ex, string userId) : base($"User modification failed at operation {operation}, {userId}", ex)
        {
            Operation = operation;
            this.userId = userId;
        }
    }
}
