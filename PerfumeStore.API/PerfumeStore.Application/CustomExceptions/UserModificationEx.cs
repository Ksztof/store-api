namespace PerfumeStore.Application.CustomExceptions
{
    public class UserModificationEx : Exception
    {
        public string Operation { get; }
        public string userId { get; }

        public UserModificationEx(string operation, string userId) : base($"User modification failed at operation {operation}, {userId}")
        {
            Operation = operation;
        }

        public UserModificationEx(string operation, Exception ex, string userId) : base($"User modification failed at operation {operation}, {userId}", ex)
        {
            Operation = operation;
            this.userId = userId;
        }
    }
}
