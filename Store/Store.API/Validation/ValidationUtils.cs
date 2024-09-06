using Store.API.Shared.Enums;

namespace Store.API.Validation;

internal static class ValidationUtils
{
    internal static bool NotContainWhiteSpace(string password)
    {
        return !password.Any(char.IsWhiteSpace);
    }

    internal static bool BeValidCategoryId(int categoryId)
    {
        return categoryId > 0;
    }

    internal static bool HaveMinimumNumberOfLetters(string password)
    {
        return password.Count(char.IsLetter) >= 4;
    }

    internal static bool HaveMinimumNumberOfDigits(string password)
    {
        return password.Count(char.IsDigit) >= 2;
    }

    internal static bool HaveAtLeastOneSpecialCharacter(string password)
    {
        return password.Any(ch => "!@#$%^&*()_+-=[]{};':\",.<>/?\\|".Contains(ch));
    }

    internal static bool BeAValidUtcDateTime(DateTime date)
    {
        return date.Kind == DateTimeKind.Utc;
    }

    internal static bool BeAValidCurrency(string currency)
    {
        var isoCurrencies = new HashSet<string>(new[] { "PLN", "USD", "EUR", "GBP" });
        return isoCurrencies.Contains(currency);
    }

    internal static bool HaveAtLeastOneLetter(string login)
    {
        return login.Any(char.IsLetter);
    }

    internal static bool StartWithLetter(string login)
    {
        return char.IsLetter(login.FirstOrDefault());
    }

    internal static bool NotContainSpecialCharactersOrWhiteSpace(string login)
    {
        return login.All(c => char.IsLetterOrDigit(c));
    }

    internal static bool BeAValidOrderMethod(string method)
    {
        return Enum.GetNames(typeof(OrderMethod)).Contains(method);
    }
}
