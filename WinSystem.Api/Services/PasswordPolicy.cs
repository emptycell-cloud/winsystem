using System.Text.RegularExpressions;

namespace WinSystem.Api.Services
{
    /// <summary>统一密码强度策略：8-64 位，必须同时包含字母和数字。</summary>
    public static class PasswordPolicy
    {
        public const string DefaultResetPassword = "Aa123456";

        public static bool IsValid(string? password) =>
            !string.IsNullOrWhiteSpace(password) &&
            password.Length >= 8 && password.Length <= 64 &&
            password.Any(char.IsLetter) && password.Any(char.IsDigit);

        public static string Error => "密码须为 8-64 位，且同时包含字母和数字。";
    }
}
