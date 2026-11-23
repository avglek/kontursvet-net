using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Kontursvet.Configuration;

/// <summary>
/// Обязательная проверка production-секретов.
/// В среде <c>Production</c> приложение не стартует, если секреты не заданы,
/// пусты или совпадают с известными значениями по умолчанию из dev-конфига.
/// </summary>
public static class ProductionSecretsGuard
{
    // Значения-заглушки из dev-конфигурации и docker-compose (публично известны в репозитории).
    private static readonly string[] KnownWeakValues =
    [
        "dev-secret-key-min-32-chars-long-change-in-prod-1234567890",
        "change-me-to-a-long-random-secret-at-least-32-chars",
        "change-me",
    ];

    private const int MinSecretLength = 32;

    /// <summary>
    /// Проверка секретов. Вызывать в <c>Program.cs</c> до <c>builder.Build()</c>.
    /// В любой среде кроме Production — ничего не делает.
    /// </summary>
    public static void EnsureProductionSecrets(IConfiguration config, IHostEnvironment environment)
    {
        if (!environment.IsProduction())
            return;

        var problems = new List<string>();

        ValidateJwtSecretKey(config, problems);
        ValidatePostgresPassword(config, problems);
        ValidateMaxToken(config, problems);
        ValidateMaxChatId(config, problems);

        if (problems.Count > 0)
        {
            var message =
                "Production-секреты не прошли проверку. Приложение не запущено:\n" +
                string.Join("\n", problems.Select((p, i) => $"  {i + 1}. {p}"));

            throw new InvalidOperationException(message);
        }
    }

    private static void ValidateJwtSecretKey(IConfiguration config, List<string> problems)
    {
        var secretKey = config["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            problems.Add("Jwt:SecretKey не задан (переменная окружения JWT_SECRET_KEY).");
            return;
        }

        if (secretKey.Length < MinSecretLength)
        {
            problems.Add(
                $"Jwt:SecretKey слишком короткий ({secretKey.Length} символов, минимум {MinSecretLength}).");
            return;
        }

        var lower = secretKey.ToLowerInvariant();

        if (KnownWeakValues.Contains(secretKey, StringComparer.OrdinalIgnoreCase))
        {
            problems.Add("Jwt:SecretKey совпадает с известным значением по умолчанию из dev-конфига.");
            return;
        }

        if (lower.Contains("change") || lower.Contains("replace") || lower.Contains("placeholder"))
        {
            problems.Add("Jwt:SecretKey содержит слова-заглушки (change/replace/placeholder).");
            return;
        }

        if (lower.Contains("dev-secret-key"))
        {
            problems.Add("Jwt:SecretKey содержит известную dev-заглушку из docker-compose.");
            return;
        }
    }

    private static void ValidatePostgresPassword(IConfiguration config, List<string> problems)
    {
        var connectionString = config.GetConnectionString("Postgres");

        if (string.IsNullOrEmpty(connectionString))
        {
            problems.Add("ConnectionStrings:Postgres не задан (переменная ConnectionStrings__Postgres).");
            return;
        }

        var password = ExtractConnectionStringValue(connectionString, "Password");

        if (string.IsNullOrEmpty(password))
        {
            problems.Add("ConnectionStrings:Postgres не содержит Password (переменная POSTGRES_PASSWORD).");
            return;
        }

        if (password.Equals("kontursvet", StringComparison.OrdinalIgnoreCase))
        {
            problems.Add(
                "ConnectionStrings:Postgres содержит пароль по умолчанию ('kontursvet'). Смените POSTGRES_PASSWORD в .env.");
            return;
        }
    }

    private static void ValidateMaxToken(IConfiguration config, List<string> problems)
    {
        var token = config["Max:Token"];

        if (string.IsNullOrEmpty(token))
        {
            problems.Add("Max:Token не задан (переменная окружения MAX_TOKEN). Без него отправка лидов в MAX не работает.");
            return;
        }

        if (token.Length < 8)
        {
            problems.Add("Max:Token слишком короткий.");
        }
    }

    private static void ValidateMaxChatId(IConfiguration config, List<string> problems)
    {
        var chatIdStr = config["Max:ChatId"];

        if (string.IsNullOrWhiteSpace(chatIdStr))
        {
            problems.Add("Max:ChatId не задан (переменная окружения MAX_CHAT_ID).");
            return;
        }

        if (!long.TryParse(chatIdStr, out _))
        {
            problems.Add($"Max:ChatId не является корректным числом ('{chatIdStr}').");
        }
    }

    /// <summary>
    /// Извлекает значение параметра из строки подключения (например Password=xxx).
    /// </summary>
    private static string? ExtractConnectionStringValue(string connectionString, string key)
    {
        var prefix = $"{key}=";
        var start = connectionString.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
            return null;

        start += prefix.Length;
        var end = connectionString.IndexOf(';', start);
        if (end < 0)
            end = connectionString.Length;

        return connectionString[start..end];
    }
}
