namespace PerfumeStore.Domain
{
    public static class ProductIdGenerator
    {
        private static int _currentId;

        public static int GetNextId() => ++_currentId;
    }
}
