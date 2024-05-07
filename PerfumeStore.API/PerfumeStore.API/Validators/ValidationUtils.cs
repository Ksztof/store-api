namespace PerfumeStore.API.Validators
{
    public static class ValidationUtils
    {
        public static bool NotContainWhiteSpace(string password)
        {
            return !password.Any(char.IsWhiteSpace);
        }

        public static bool BeValidCategoryId(int categoryId)
        {
            return categoryId > 0;
        }

        public static bool HaveMinimumNumberOfLetters(string password)
        {
            return password.Count(char.IsLetter) >= 4;
        }

        public static bool HaveMinimumNumberOfDigits(string password)
        {
            return password.Count(char.IsDigit) >= 2;
        }

        public static bool HaveAtLeastOneSpecialCharacter(string password)
        {
            return password.Any(ch => "!@#$%^&*()_+-=[]{};':\",.<>/?\\|".Contains(ch));
        }

        public static bool BeAValidUtcDateTime(DateTime date)
        {
            return date.Kind == DateTimeKind.Unspecified;
        }
    }
}
