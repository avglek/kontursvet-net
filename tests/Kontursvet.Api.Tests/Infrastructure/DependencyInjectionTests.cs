using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Abstractions.Users;
using Kontursvet.Infrastructure;
using Kontursvet.Infrastructure.Data;
using Kontursvet.Infrastructure.Messaging;
using Kontursvet.Infrastructure.Repositories;
using Kontursvet.Infrastructure.Security;
using Kontursvet.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kontursvet.Api.Tests.Infrastructure;

/// <summary>
/// Тесты <see cref="DependencyInjection.AddInfrastructure"/>: Infrastructure DI
/// без запуска реального PostgreSQL и без сетевых вызовов MAX.
/// </summary>
public sealed class DependencyInjectionTests
{
    private static IConfiguration Config(Dictionary<string, string?>? overrides = null)
    {
        var data = new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres"] = "Host=localhost;Database=test;Username=u;Password=p",
            ["Jwt:SecretKey"] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            ["Storage:LocalRoot"] = Path.GetTempPath(),
            ["Max:Token"] = "test-token",
            ["Max:ChatId"] = "-78888336665234"
        };

        if (overrides is not null)
            foreach (var (key, value) in overrides)
                data[key] = value;

        return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
    }

    // ==================== Успешная регистрация ====================

    [Fact]
    public void AddInfrastructure_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var config = Config();

        var result = services.AddInfrastructure(config);

        Assert.Same(services, result);
    }

    [Fact]
    public void AddInfrastructure_RegistersEveryExpectedAbstraction()
    {
        var services = new ServiceCollection();
        var config = Config();

        services.AddInfrastructure(config);

        // Репозитории и фабрика соединений — scoped (соединение на запрос)
        AssertSingle(services, typeof(IDbConnectionFactory), ServiceLifetime.Singleton, null);
        AssertSingle(services, typeof(IPortfolioRepository), ServiceLifetime.Scoped, typeof(PortfolioRepository));
        AssertSingle(services, typeof(ILeadRepository), ServiceLifetime.Scoped, typeof(LeadRepository));
        AssertSingle(services, typeof(IProductRepository), ServiceLifetime.Scoped, typeof(ProductRepository));
        AssertSingle(services, typeof(IAdminUserRepository), ServiceLifetime.Scoped, typeof(AdminUserRepository));

        // Безопасность и хранилище — синглтоны (stateless)
        AssertSingle(services, typeof(IPasswordHasher), ServiceLifetime.Singleton, typeof(BCryptPasswordHasher));
        AssertSingle(services, typeof(IJwtTokenService), ServiceLifetime.Singleton, typeof(JwtTokenService));
        AssertSingle(services, typeof(IFileStorage), ServiceLifetime.Singleton, typeof(LocalFileStorage));

        // MAX: один и тот же клиент под тремя интерфейсами
        AssertSingle(services, typeof(MaxMessageDispatcher), ServiceLifetime.Singleton, typeof(MaxMessageDispatcher));
        Assert.Contains(services, d =>
            d.ServiceType == typeof(IMessageDispatcher) && d.Lifetime == ServiceLifetime.Singleton);
        Assert.Contains(services, d =>
            d.ServiceType == typeof(IUploadLeadFile) && d.Lifetime == ServiceLifetime.Singleton);
    }

    // ==================== Повторный вызов ====================

    [Fact]
    public void AddInfrastructure_CalledTwice_RegistersDuplicates()
    {
        // Повторный вызов НЕ запрещён — но он добавляет дескрипторы заново.
        // В DI это допустимо: GetRequiredService вернёт последний.
        // Тест фиксирует текущее поведение.
        var services = new ServiceCollection();
        var config = Config();

        services.AddInfrastructure(config);
        services.AddInfrastructure(config);

        Assert.Equal(2, Count(services, typeof(IPortfolioRepository)));
    }

    // ==================== Строка подключения ====================

    [Fact]
    public void AddInfrastructure_MissingConnectionString_Throws()
    {
        var empty = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var services = new ServiceCollection();

        var ex = Assert.Throws<InvalidOperationException>(() => services.AddInfrastructure(empty));

        Assert.Contains("ConnectionStrings:Postgres", ex.Message);
    }

    [Fact]
    public void AddInfrastructure_BlankConnectionString_IsAccepted()
    {
        // Фиксируем фактическое поведение: проверка «строка задана» делает null-проверку,
        // поэтому пустая строка проходит дальше — за содержимое отвечает ProductionSecretsGuard
        var config = Config(new() { ["ConnectionStrings:Postgres"] = "" });
        var services = new ServiceCollection();

        services.AddInfrastructure(config);

        Assert.Contains(services, d => d.ServiceType == typeof(IDbConnectionFactory));
    }

    [Fact]
    public void AddInfrastructure_ThrowsBeforeAnyRegistration()
    {
        var empty = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var services = new ServiceCollection();

        Record.Exception(() => services.AddInfrastructure(empty));

        // Ничего не должно быть добавлено до падения
        Assert.Empty(services);
    }

    // ==================== Разрешение из контейнера ====================

    [Fact]
    public void ResolvesAllSingletons()
    {
        using var provider = BuildProvider();

        Assert.IsType<NpgsqlConnectionFactory>(provider.GetRequiredService<IDbConnectionFactory>());
        Assert.IsType<BCryptPasswordHasher>(provider.GetRequiredService<IPasswordHasher>());
        Assert.IsType<JwtTokenService>(provider.GetRequiredService<IJwtTokenService>());
        Assert.IsType<LocalFileStorage>(provider.GetRequiredService<IFileStorage>());
    }

    [Fact]
    public void ResolvesAllScopedRepositoriesInScope()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;

        Assert.IsType<PortfolioRepository>(sp.GetRequiredService<IPortfolioRepository>());
        Assert.IsType<LeadRepository>(sp.GetRequiredService<ILeadRepository>());
        Assert.IsType<ProductRepository>(sp.GetRequiredService<IProductRepository>());
        Assert.IsType<AdminUserRepository>(sp.GetRequiredService<IAdminUserRepository>());
    }

    [Fact]
    public void ResolvesMaxMessageDispatcher_AllInterfacesToSameInstance()
    {
        using var provider = BuildProvider();

        var viaConcrete = provider.GetRequiredService<MaxMessageDispatcher>();
        var viaDispatcher = provider.GetRequiredService<IMessageDispatcher>();
        var viaUpload = provider.GetRequiredService<IUploadLeadFile>();

        Assert.Same(viaConcrete, viaDispatcher);
        Assert.Same(viaConcrete, viaUpload);
    }

    [Fact]
    public void ScopedRepositories_DifferentInstancesPerScope()
    {
        using var provider = BuildProvider();
        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();

        var repo1 = scope1.ServiceProvider.GetRequiredService<IPortfolioRepository>();
        var repo2 = scope2.ServiceProvider.GetRequiredService<IPortfolioRepository>();

        Assert.NotSame(repo1, repo2);
    }

    [Fact]
    public void SingletonServices_SameAcrossScopes()
    {
        using var provider = BuildProvider();
        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();

        var h1 = scope1.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var h2 = scope2.ServiceProvider.GetRequiredService<IPasswordHasher>();

        Assert.Same(h1, h2);
    }

    // ==================== Хелперы ====================

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(Config());
        services.AddLogging();
        services.AddInfrastructure(Config());
        return services.BuildServiceProvider();
    }

    private static void AssertSingle(
        IServiceCollection services,
        Type serviceType,
        ServiceLifetime expectedLifetime,
        Type? expectedImplementation)
    {
        var descriptor = Assert.Single(services, d => d.ServiceType == serviceType);
        Assert.Equal(expectedLifetime, descriptor.Lifetime);
        if (expectedImplementation is not null)
            Assert.Equal(expectedImplementation, descriptor.ImplementationType);
    }

    private static int Count(IServiceCollection services, Type serviceType)
        => services.Count(d => d.ServiceType == serviceType);
}
