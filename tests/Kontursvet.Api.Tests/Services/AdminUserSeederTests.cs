using Kontursvet.Api.Services;
using Kontursvet.Application.Abstractions.Users;
using Kontursvet.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Kontursvet.Api.Tests.Services;

/// <summary>
/// Тесты <see cref="AdminUserSeeder"/>: посев только при пустой таблице,
/// fallback в Development, fail-fast в Production, передача username/role из конфигурации.
/// </summary>
public class AdminUserSeederTests
{
    private static IConfiguration Config(Dictionary<string, string?>? overrides = null)
    {
        var data = overrides ?? new Dictionary<string, string?>();
        return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
    }

    private static IHostEnvironment Env(string name) => new FakeHostEnvironment(name);

    private static (AdminUserSeeder Seeder, FakeAdminUserRepository Repo, FakePasswordHasher Hasher) Build(
        IConfiguration config,
        IHostEnvironment env,
        bool hasAnyAdmin = false,
        Func<Exception>? anyThrow = null)
    {
        var repo = new FakeAdminUserRepository(hasAnyAdmin, anyThrow);
        var hasher = new FakePasswordHasher();

        var services = new ServiceCollection();
        services.AddSingleton<IAdminUserRepository>(repo);
        services.AddSingleton<IPasswordHasher>(hasher);
        services.AddSingleton(NullLogger<AdminUserSeeder>.Instance);

        var provider = services.BuildServiceProvider();
        var seeder = new AdminUserSeeder(provider, config, env, NullLogger<AdminUserSeeder>.Instance);

        return (seeder, repo, hasher);
    }

    // ==================== Пропуск при непустой таблице ====================

    [Fact]
    public async Task StartAsync_TableNotEmpty_DoesNotCreate()
    {
        var (seeder, repo, hasher) = Build(Config(), Env("Production"), hasAnyAdmin: true);

        await seeder.StartAsync(CancellationToken.None);

        Assert.Null(repo.Created);
        Assert.Empty(hasher.Hashed);
    }

    [Fact]
    public async Task StartAsync_TableNotEmpty_ProductionWithoutPassword_DoesNotThrow()
    {
        // Админ уже есть → требование пароля неактуально
        var (seeder, repo, _) = Build(Config(), Env("Production"), hasAnyAdmin: true);

        await seeder.StartAsync(CancellationToken.None);

        Assert.Null(repo.Created);
    }

    [Fact]
    public async Task StartAsync_TableNotEmpty_DoesNotEvenReadPassword()
    {
        var (seeder, repo, _) = Build(
            Config(new() { ["Admin:Password"] = "weak-but-ignored" }),
            Env("Production"),
            hasAnyAdmin: true);

        await seeder.StartAsync(CancellationToken.None);

        Assert.Null(repo.Created);
    }

    // ==================== Development: fallback ====================

    [Fact]
    public async Task StartAsync_Development_EmptyPassword_UsesFallback()
    {
        var (seeder, repo, hasher) = Build(Config(), Env("Development"));

        await seeder.StartAsync(CancellationToken.None);

        Assert.NotNull(repo.Created);
        Assert.Equal("admin", repo.Created!.Username);
        Assert.Equal("Admin", repo.Created.Role);
        Assert.Single(hasher.Hashed);
        Assert.Equal("admin123", hasher.Hashed[0]);
    }

    [Fact]
    public async Task StartAsync_Development_WeakPassword_IsAllowed()
    {
        var (seeder, repo, hasher) = Build(
            Config(new() { ["Admin:Password"] = "abc" }),
            Env("Development"));

        await seeder.StartAsync(CancellationToken.None);

        Assert.NotNull(repo.Created);
        Assert.Equal("abc", hasher.Hashed[0]);
    }

    [Fact]
    public async Task StartAsync_Development_WithStrongPassword_UsesIt()
    {
        var (seeder, repo, hasher) = Build(
            Config(new()
            {
                ["Admin:Username"] = "owner",
                ["Admin:Password"] = "Str0ng-Pa55-w0rd",
                ["Admin:Role"] = "Editor"
            }),
            Env("Development"));

        await seeder.StartAsync(CancellationToken.None);

        Assert.NotNull(repo.Created);
        Assert.Equal("owner", repo.Created!.Username);
        Assert.Equal("Editor", repo.Created.Role);
        Assert.Equal("Str0ng-Pa55-w0rd", hasher.Hashed[0]);
    }

    // ==================== Production: fail-fast ====================

    [Fact]
    public async Task StartAsync_Production_EmptyPassword_Throws()
    {
        var (seeder, repo, _) = Build(Config(), Env("Production"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.StartAsync(CancellationToken.None));

        Assert.Contains("ADMIN_PASSWORD", ex.Message);
        Assert.Null(repo.Created);
    }

    [Fact]
    public async Task StartAsync_Production_DefaultAdmin123_Throws()
    {
        var (seeder, repo, _) = Build(
            Config(new() { ["Admin:Password"] = "admin123" }),
            Env("Production"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.StartAsync(CancellationToken.None));

        Assert.Contains("слабый пароль", ex.Message);
        Assert.Null(repo.Created);
    }

    [Fact]
    public async Task StartAsync_Production_ShortPassword_Throws()
    {
        var (seeder, repo, _) = Build(
            Config(new() { ["Admin:Password"] = "1234567" }),   // 7 символов
            Env("Production"));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.StartAsync(CancellationToken.None));

        Assert.Contains("слабый пароль", ex.Message);
        Assert.Null(repo.Created);
    }

    [Fact]
    public async Task StartAsync_Production_StrongPassword_CreatesAdmin()
    {
        var (seeder, repo, hasher) = Build(
            Config(new()
            {
                ["Admin:Username"] = "kontursvet-admin",
                ["Admin:Password"] = "V3ryStr0ng!Passphrase",
                ["Admin:Role"] = "Admin"
            }),
            Env("Production"));

        await seeder.StartAsync(CancellationToken.None);

        Assert.NotNull(repo.Created);
        Assert.Equal("kontursvet-admin", repo.Created!.Username);
        Assert.Equal("Admin", repo.Created.Role);
        Assert.Single(hasher.Hashed);
        Assert.Equal("V3ryStr0ng!Passphrase", hasher.Hashed[0]);
        // Пароль не хранится в открытом виде — только хэш
        Assert.NotEqual("V3ryStr0ng!Passphrase", repo.Created.PasswordHash);
    }

    [Fact]
    public async Task StartAsync_Production_BoundaryEightCharsPassword_IsAllowed()
    {
        var (seeder, repo, _) = Build(
            Config(new() { ["Admin:Password"] = "12345678" }),   // ровно 8
            Env("Production"));

        await seeder.StartAsync(CancellationToken.None);

        Assert.NotNull(repo.Created);
    }

    // ==================== Username: default / trim ====================

    [Theory]
    [InlineData(null, "admin")]
    [InlineData("", "admin")]
    [InlineData("   ", "admin")]
    [InlineData("  owner  ", "owner")]
    public async Task StartAsync_UsernameNormalization(string? configured, string expected)
    {
        var (seeder, repo, _) = Build(
            Config(new()
            {
                ["Admin:Username"] = configured,
                ["Admin:Password"] = "Str0ng-Pa55-w0rd"
            }),
            Env("Development"));

        await seeder.StartAsync(CancellationToken.None);

        Assert.NotNull(repo.Created);
        Assert.Equal(expected, repo.Created!.Username);
    }

    [Fact]
    public async Task StartAsync_RoleDefault_IsAdmin()
    {
        var (seeder, repo, _) = Build(
            Config(new() { ["Admin:Password"] = "Str0ng-Pa55-w0rd" }),
            Env("Development"));

        await seeder.StartAsync(CancellationToken.None);

        Assert.Equal("Admin", repo.Created!.Role);
    }

    // ==================== Поведение при недоступной БД ====================

    [Fact]
    public async Task StartAsync_Development_DatabaseUnavailable_SkipsSilently()
    {
        var (seeder, repo, _) = Build(
            Config(new() { ["Admin:Password"] = "Str0ng-Pa55-w0rd" }),
            Env("Development"),
            anyThrow: () => new InvalidOperationException("connection refused"));

        // В Development исключение проглатывается (БД может быть не готова)
        await seeder.StartAsync(CancellationToken.None);

        Assert.Null(repo.Created);
    }

    [Fact]
    public async Task StartAsync_Production_DatabaseUnavailable_Rethrows()
    {
        var (seeder, repo, _) = Build(
            Config(new() { ["Admin:Password"] = "Str0ng-Pa55-w0rd" }),
            Env("Production"),
            anyThrow: () => new InvalidOperationException("connection refused"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => seeder.StartAsync(CancellationToken.None));
        Assert.Null(repo.Created);
    }

    // ==================== StopAsync ====================

    [Fact]
    public async Task StopAsync_Completes()
    {
        var (seeder, _, _) = Build(Config(), Env("Development"));

        await seeder.StopAsync(CancellationToken.None);
    }

    // ==================== Fakes ====================

    private sealed class FakeAdminUserRepository(bool hasAny, Func<Exception>? anyThrow) : IAdminUserRepository
    {
        public AdminUser? Created { get; private set; }

        public Task<bool> AnyAsync(CancellationToken ct)
        {
            if (anyThrow is not null)
                throw anyThrow();

            return Task.FromResult(hasAny);
        }

        public Task<long> CreateAsync(AdminUser user, CancellationToken ct)
        {
            Created = user;
            return Task.FromResult(1L);
        }

        public Task<AdminUser?> GetByUsernameAsync(string username, CancellationToken ct) =>
            Task.FromResult<AdminUser?>(null);

        public Task<AdminUser?> GetByIdAsync(long id, CancellationToken ct) =>
            Task.FromResult<AdminUser?>(null);
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public List<string> Hashed { get; } = [];

        public string Hash(string password)
        {
            Hashed.Add(password);
            return $"HASH({password})";
        }

        public bool Verify(string password, string hash) => hash == $"HASH({password})";
    }

    private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "Kontursvet.Api";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
