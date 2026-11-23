using Kontursvet.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Kontursvet.Api.Tests.Confuguration;

/// <summary>
/// Тесты <see cref="ProductionSecretsGuard"/>: в Production пустые, известные и слабые
/// секреты блокируют запуск; вне Production проверка не выполняется.
/// </summary>
public class ProductionSecretsGuardTests
{
    // Валидный «скелет» конфига: все секреты хороши, кроме тех, что подменяются в тестах
    private const string GoodJwt = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";   // 32 символа
    private const string GoodCs = "Host=postgres;Database=k;Username=k;Password=Str0ngPass";
    private const string GoodToken = "max-token-value";
    private const string GoodChatId = "-78888336665234";

    private static IConfiguration Config(Dictionary<string, string?>? overrides = null)
    {
        var data = new Dictionary<string, string?>
        {
            ["Jwt:SecretKey"] = GoodJwt,
            ["ConnectionStrings:Postgres"] = GoodCs,
            ["Max:Token"] = GoodToken,
            ["Max:ChatId"] = GoodChatId
        };

        if (overrides is not null)
            foreach (var (key, value) in overrides)
                data[key] = value;

        return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
    }

    private static IHostEnvironment Env(string name) => new FakeHostEnvironment(name);

    private static InvalidOperationException Throws(IConfiguration config, string environment = "Production") =>
        Assert.Throws<InvalidOperationException>(() =>
            ProductionSecretsGuard.EnsureProductionSecrets(config, Env(environment)));

    // ==================== Не Production — проверка пропускается ====================

    [Theory]
    [InlineData("Development")]
    [InlineData("Staging")]
    [InlineData("Testing")]
    public void NonProduction_EmptyConfig_DoesNotThrow(string environment)
    {
        var empty = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

        ProductionSecretsGuard.EnsureProductionSecrets(empty, Env(environment));
    }

    // ==================== Production: пустой конфиг ====================

    [Fact]
    public void Production_EmptyConfig_ListsEveryMissingSecret()
    {
        var empty = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

        var ex = Throws(empty);

        Assert.Contains("Jwt:SecretKey", ex.Message);
        Assert.Contains("ConnectionStrings:Postgres", ex.Message);
        Assert.Contains("Max:Token", ex.Message);
        Assert.Contains("Max:ChatId", ex.Message);
        Assert.Contains("не прошли проверку", ex.Message);
    }

    // ==================== Production: валидный конфиг ====================

    [Fact]
    public void Production_AllSecretsValid_DoesNotThrow()
    {
        ProductionSecretsGuard.EnsureProductionSecrets(Config(), Env("Production"));
    }

    [Fact]
    public void Production_AllSecretsValid_AndStrongAdminPassword_DoesNotThrow()
    {
        var config = Config(new() { ["Admin:Password"] = "Str0ng-Pa55-w0rd" });

        ProductionSecretsGuard.EnsureProductionSecrets(config, Env("Production"));
    }

    // ==================== Jwt:SecretKey ====================

    [Fact]
    public void Production_JwtSecretKeyMissing_Throws()
    {
        var config = Config(new() { ["Jwt:SecretKey"] = null });

        Assert.Contains("Jwt:SecretKey не задан", Throws(config).Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Production_JwtSecretKeyBlank_Throws(string key)
    {
        var config = Config(new() { ["Jwt:SecretKey"] = key });

        Assert.Contains("Jwt:SecretKey", Throws(config).Message);
    }

    [Fact]
    public void Production_JwtSecretKeyTooShort_Throws()
    {
        var config = Config(new() { ["Jwt:SecretKey"] = "short-key-32" });

        Assert.Contains("слишком короткий", Throws(config).Message);
    }

    [Theory]
    [InlineData("dev-secret-key-min-32-chars-long-change-in-prod-1234567890")]
    [InlineData("change-me-to-a-long-random-secret-at-least-32-chars")]
    public void Production_JwtSecretKeyKnownPlaceholder_Throws(string key)
    {
        var config = Config(new() { ["Jwt:SecretKey"] = key });

        Assert.Contains("известным значением по умолчанию", Throws(config).Message);
    }

    [Theory]
    [InlineData("change-me-please-this-secret-key-is-32-plus")]
    [InlineData("REPLACE_WITH_REAL_SECRET_KEY_VALUE_1234567890")]
    [InlineData("placeholder-secret-key-value-that-is-long-enough")]
    public void Production_JwtSecretKeyContainsPlaceholderWord_Throws(string key)
    {
        var config = Config(new() { ["Jwt:SecretKey"] = key });

        Assert.Contains("слова-заглушки", Throws(config).Message);
    }

    [Fact]
    public void Production_JwtSecretKeyContainsDevSecretKeyMarker_Throws()
    {
        // Достаточно длинный ключ, в котором нет change/replace/placeholder,
        // но есть метка dev-secret-key из docker-compose-заглушки
        var config = Config(new() { ["Jwt:SecretKey"] = "dev-secret-key-9f3a1c7b5e2d8a4f6c1b3e5d7a9f2c" });

        Assert.Contains("docker-compose", Throws(config).Message);
    }

    // ==================== ConnectionStrings:Postgres ====================

    [Fact]
    public void Production_ConnectionStringMissing_Throws()
    {
        var config = Config(new() { ["ConnectionStrings:Postgres"] = null });

        Assert.Contains("ConnectionStrings:Postgres не задан", Throws(config).Message);
    }

    [Fact]
    public void Production_ConnectionStringWithoutPassword_Throws()
    {
        var config = Config(new() { ["ConnectionStrings:Postgres"] = "Host=postgres;Database=k;Username=k" });

        Assert.Contains("не содержит Password", Throws(config).Message);
    }

    [Fact]
    public void Production_ConnectionStringDefaultPassword_Throws()
    {
        var config = Config(new() { ["ConnectionStrings:Postgres"] = "Host=postgres;Database=k;Username=k;Password=kontursvet" });

        var ex = Throws(config);
        Assert.Contains("по умолчанию", ex.Message);
        Assert.Contains("POSTGRES_PASSWORD", ex.Message);
    }

    [Fact]
    public void Production_ConnectionStringDefaultPassword_CaseInsensitive_Throws()
    {
        var config = Config(new() { ["ConnectionStrings:Postgres"] = "Host=postgres;password=KONTURSVET" });

        Assert.Contains("по умолчанию", Throws(config).Message);
    }

    // ==================== Max:Token / Max:ChatId ====================

    [Fact]
    public void Production_MaxTokenMissing_Throws()
    {
        var config = Config(new() { ["Max:Token"] = null });

        Assert.Contains("Max:Token не задан", Throws(config).Message);
    }

    [Fact]
    public void Production_MaxTokenTooShort_Throws()
    {
        var config = Config(new() { ["Max:Token"] = "abc" });

        Assert.Contains("Max:Token слишком короткий", Throws(config).Message);
    }

    [Fact]
    public void Production_MaxChatIdMissing_Throws()
    {
        var config = Config(new() { ["Max:ChatId"] = "" });

        Assert.Contains("Max:ChatId не задан", Throws(config).Message);
    }

    [Theory]
    [InlineData("not-a-number")]
    [InlineData("12.5")]
    public void Production_MaxChatIdNotInteger_Throws(string chatId)
    {
        var config = Config(new() { ["Max:ChatId"] = chatId });

        Assert.Contains("не является корректным числом", Throws(config).Message);
    }

    // ==================== Admin:Password ====================

    [Fact]
    public void Production_AdminPasswordNotSet_IsNotFlagged()
    {
        // Пароль не задан → guard молчит (за него отвечает AdminUserSeeder)
        ProductionSecretsGuard.EnsureProductionSecrets(Config(), Env("Production"));
    }

    [Theory]
    [InlineData("admin123")]
    [InlineData("ADMIN123")]
    public void Production_AdminPasswordDefault_Throws(string password)
    {
        var config = Config(new() { ["Admin:Password"] = password });

        Assert.Contains("admin123", Throws(config).Message);
    }

    [Fact]
    public void Production_AdminPasswordTooShort_Throws()
    {
        var config = Config(new() { ["Admin:Password"] = "abc" });

        Assert.Contains("Admin:Password слишком короткий", Throws(config).Message);
    }

    [Fact]
    public void Production_AdminPasswordExactly8Chars_IsAccepted()
    {
        var config = Config(new() { ["Admin:Password"] = "12345678" });

        ProductionSecretsGuard.EnsureProductionSecrets(config, Env("Production"));
    }

    // ==================== Несколько ошибок сразу ====================

    [Fact]
    public void Production_MultipleWeakSecrets_ReportsAllOfThem()
    {
        var config = Config(new()
        {
            ["Jwt:SecretKey"] = "dev-secret-key-min-32-chars-long-change-in-prod-1234567890",
            ["ConnectionStrings:Postgres"] = "Host=postgres;Database=k;Username=k;Password=kontursvet",
            ["Max:Token"] = null,
            ["Admin:Password"] = "admin123"
        });

        var ex = Throws(config);

        Assert.Contains("Jwt:SecretKey", ex.Message);
        Assert.Contains("ConnectionStrings:Postgres", ex.Message);
        Assert.Contains("Max:Token", ex.Message);
        Assert.Contains("Admin:Password", ex.Message);
    }

    // ==================== Заглушки ====================

    private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "Kontursvet.Api";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
