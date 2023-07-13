namespace PerfumeStore.Domain
{
    public static class CartIdGenerator
    {
        private static int _currentId;

        public static int GetNextId() => ++_currentId;
    }
}
