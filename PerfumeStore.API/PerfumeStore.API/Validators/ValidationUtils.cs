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
            return date.Kind == DateTimeKind.Utc;
        }

        public static bool BeAValidCurrency(string currency)
        {
            var isoCurrencies = new HashSet<string>(new[] { "PLN", "USD", "EUR", "GBP" }); 
            return isoCurrencies.Contains(currency);
        }

        public static bool HaveAtLeastOneLetter(string login)
        {
            return login.Any(char.IsLetter);
        }

        public static bool StartWithLetter(string login)
        {
            return char.IsLetter(login.FirstOrDefault());
        }

        public static bool NotContainSpecialCharactersOrWhiteSpace(string login)
        {
            return login.All(c => char.IsLetterOrDigit(c));
        }
    }
}
