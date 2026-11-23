using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Abstractions.Users;
using Kontursvet.Application.Features.Auth;
using Kontursvet.Application.Features.Files;
using Kontursvet.Application.Features.Leads;
using Kontursvet.Application.Features.Portfolio.Card;
using Kontursvet.Application.Features.Portfolio.CardView;
using Kontursvet.Application.Features.Portfolio.Gallery;
using Kontursvet.Application.Features.Products;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Kontursvet.Application.Tests;

/// <summary>
/// Тесты <see cref="DependencyInjection.AddApplication"/>: все обработчики зарегистрированы
/// как Scoped и реально разрешаются контейнером.
/// </summary>
public class DependencyInjectionTests
{
    /// <summary>
    /// Полный список обработчиков, которые обязан регистрировать Application-слой.
    /// Если в <c>AddApplication</c> пропустить new сценарий — этот массив рассинхронизируется
    /// с тестом <see cref="EveryHandler_IsRegisteredAsScopedWithItsOwnType"/>, и тест упадёт.
    /// </summary>
    private static readonly Type[] AllHandlers =
    [
        // Products
        typeof(CreateProductHandler),
        typeof(GetAllProductsHandler),
        typeof(GetProductByIdHandler),
        typeof(UpdateProductHandler),
        typeof(DeleteProductHandler),

        // PortfolioCard
        typeof(CreatePortfolioCardHandler),
        typeof(GetPortfolioCardHandler),
        typeof(GetPortfolioCardsHandler),
        typeof(UpdatePortfolioCardHandler),
        typeof(DeletePortfolioCardHandler),
        typeof(GetPortfolioCardsCountHandler),
        typeof(GetPortfolioPhotosCountHandler),

        // PortfolioCardView
        typeof(GetPortfolioCardViewHandler),
        typeof(GetPortfolioCardViewsHandler),
        typeof(UpdatePortfolioCardViewHandler),
        typeof(DeletePortfolioCardViewHandler),

        // Gallery
        typeof(AddGalleryItemHandler),
        typeof(UpdateGalleryItemHandler),
        typeof(DeleteGalleryItemHandler),

        // Leads
        typeof(SendLeadHandler),
        typeof(UploadLeadFileHandler),

        // Auth
        typeof(LoginHandler),

        // Files
        typeof(UploadFileHandler)
    ];

    public static TheoryData<Type> HandlerTypes { get; } = new(AllHandlers);

    // ==================== Регистрация ====================

    [Fact]
    public void AddApplication_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApplication();

        Assert.Same(services, result);
    }

    [Theory]
    [MemberData(nameof(HandlerTypes))]
    public void EveryHandler_IsRegisteredAsScopedWithItsOwnType(Type handler)
    {
        var services = new ServiceCollection();

        services.AddApplication();

        var descriptor = Assert.Single(services, d => d.ServiceType == handler);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(handler, descriptor.ImplementationType);
    }

    [Fact]
    public void AddApplication_RegistersExactlyTheHandlerSet_NothingElse()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        Assert.Equal(AllHandlers.Length, services.Count);
        Assert.All(services, d => Assert.Contains(d.ServiceType, AllHandlers));
    }

    [Fact]
    public void AddApplication_DoesNotRegisterAbstractions()
    {
        // Интерфейсы-абстракции реализует Infrastructure, а не Application
        var services = new ServiceCollection();

        services.AddApplication();

        Type[] abstractions =
        [
            typeof(IPortfolioRepository),
            typeof(IProductRepository),
            typeof(ILeadRepository),
            typeof(IAdminUserRepository),
            typeof(IMessageDispatcher),
            typeof(IFileStorage),
            typeof(IUploadLeadFile),
            typeof(IPasswordHasher),
            typeof(IJwtTokenService)
        ];

        Assert.All(abstractions, a =>
            Assert.DoesNotContain(services, d => d.ServiceType == a));
    }

    [Fact]
    public void AddApplication_CalledTwice_DoesNotThrow()
    {
        var services = new ServiceCollection();

        services.AddApplication();
        var ex = Record.Exception(() => services.AddApplication());

        Assert.Null(ex);
    }

    // ==================== Разрешение из контейнера ====================

    [Theory]
    [MemberData(nameof(HandlerTypes))]
    public void EveryHandler_IsResolvableWithinScope(Type handler)
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var resolved = scope.ServiceProvider.GetRequiredService(handler);

        Assert.IsAssignableFrom(handler, resolved);
        // Scoped-обработчик в разных скоупах — разные экземпляры
        using var otherScope = provider.CreateScope();
        Assert.NotEqual(resolved, otherScope.ServiceProvider.GetRequiredService(handler));
    }

    [Fact]
    public void Handlers_DependOnlyOnAbstractions()
    {
        // У каждого обработчика все параметры конструктора — интерфейсы из абстракций
        // (проверка, что Application-слой не тянет конкретику вроде HttpClient или DbContext)
        foreach (var handler in HandlerTypes)
        {
            var ctor = handler.GetConstructors().Single();
            Assert.All(ctor.GetParameters(), p =>
            {
                Assert.True(p.ParameterType.IsInterface,
                    $"{handler.Name}({p.Name}) зависит от конкретного типа {p.ParameterType.Name}");
                Assert.StartsWith("Kontursvet.Application.Abstractions", p.ParameterType.Namespace);
            });
        }
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        services.AddSingleton(Mock.Of<IPortfolioRepository>());
        services.AddSingleton(Mock.Of<IProductRepository>());
        services.AddSingleton(Mock.Of<ILeadRepository>());
        services.AddSingleton(Mock.Of<IAdminUserRepository>());
        services.AddSingleton(Mock.Of<IMessageDispatcher>());
        services.AddSingleton(Mock.Of<IFileStorage>());
        services.AddSingleton(Mock.Of<IUploadLeadFile>());
        services.AddSingleton(Mock.Of<IPasswordHasher>());
        services.AddSingleton(Mock.Of<IJwtTokenService>());

        services.AddApplication();

        return services.BuildServiceProvider(validateScopes: true);
    }
}
