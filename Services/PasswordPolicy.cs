namespace NorviaBackoffice.Services;
public static class PasswordPolicy
{
    private static readonly string[] Common = ["password1234", "123456789012", "qwerty123456", "admin1234567", "letmein123456"];
    public static string? Validate(string password)
    {
        if (password.Length < 12) return "Use at least 12 characters.";
        if (Common.Contains(password, StringComparer.OrdinalIgnoreCase)) return "Choose a less common password.";
        return null;
    }
}
