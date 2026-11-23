using Kontursvet.Api.Services;
using Kontursvet.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;

namespace Kontursvet.Api.Tests.Confuguration;

/// <summary>
/// Тесты <see cref="ApiServiceExtensions.AddApiServices"/>: JWT-параметры,
/// CORS, camelCase JSON, посев администратора как hosted service.
/// HTTP-конвейер не поднимается — проверяется только то, что попадает в DI-контейнер.
/// </summary>
public sealed class ApiServiceExtensionsTests
{
    private const string JwtSecret = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private static IConfiguration Config(Dictionary<string, string?>? overrides = null)
    {
        var data = new Dictionary<string, string?>
        {
            ["Jwt:SecretKey"] = JwtSecret,
            ["Jwt:Issuer"] = "kontursvet",
            ["Jwt:Audience"] = "kontursvet-admin",
            ["ConnectionStrings:Postgres"] = "Host=localhost;Database=k;Username=u;Password=p",
            ["Max:Token"] = "max-token-value",
            ["Max:ChatId"] = "-78888336665234"
        };

        if (overrides is not null)
            foreach (var (key, value) in overrides)
                data[key] = value;

        return new ConfigurationBuilder().AddInMemoryCollection(data).Build();
    }

    private static ServiceProvider BuildProvider(IConfiguration config)
    {
        var services = new ServiceCollection();
        services.AddSingleton(config);
        services.AddSingleton<IWebHostEnvironment>(new FakeWebHostEnvironment());
        services.AddLogging();
        services.AddOptions();
        services.AddApiServices(config);
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// SwaggerGen тянет IWebHostEnvironment для поиска XML-комментариев — без него
    /// ISwaggerProvider не разрешается. В тестах окружение фиктивное.
    /// </summary>
    private sealed class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Testing";
        public string ApplicationName { get; set; } = "Kontursvet.Api";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; }
            = new Microsoft.Extensions.FileProviders.NullFileProvider();
        public string? WebRootPath { get; set; }
        public Microsoft.Extensions.FileProviders.IFileProvider? WebRootFileProvider { get; set; }
    }

    // ==================== Контракт метода ====================

    [Fact]
    public void AddApiServices_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApiServices(Config());

        Assert.Same(services, result);
    }

    [Fact]
    public void AddApiServices_MissingJwtSecret_Throws()
    {
        var config = Config(new() { ["Jwt:SecretKey"] = null });
        var services = new ServiceCollection();

        var ex = Assert.Throws<InvalidOperationException>(() => services.AddApiServices(config));

        Assert.Contains("Jwt:SecretKey", ex.Message);
    }

    [Fact]
    public void AddApiServices_MissingJwtSecret_RegistersNothing()
    {
        var config = Config(new() { ["Jwt:SecretKey"] = null });
        var services = new ServiceCollection();

        Record.Exception(() => services.AddApiServices(config));

        Assert.Empty(services);
    }

    // ==================== JWT ====================

    [Fact]
    public async Task AddApiServices_ConfiguresJwtBearerAsDefaultScheme()
    {
        using var provider = BuildProvider(Config());

        var scheme = await provider
            .GetRequiredService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>()
            .GetDefaultAuthenticateSchemeAsync();

        Assert.NotNull(scheme);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, scheme!.Name);

        // Параметры валидации привязаны к схеме "Bearer", а не к глобальной схеме
        var parameters = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;

        Assert.Equal("kontursvet", parameters.ValidIssuer);
        Assert.Equal("kontursvet-admin", parameters.ValidAudience);
    }

    [Fact]
    public void AddApiServices_JwtBearerOptions_HaveStrictTokenValidation()
    {
        using var provider = BuildProvider(Config());

        var parameters = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;

        Assert.True(parameters.ValidateIssuer);
        Assert.True(parameters.ValidateAudience);
        Assert.True(parameters.ValidateLifetime);
        Assert.True(parameters.ValidateIssuerSigningKey);
        Assert.Equal(TimeSpan.Zero, parameters.ClockSkew);
    }

    [Fact]
    public void AddApiServices_JwtBearerOptions_TakeIssuerAndAudienceFromConfig()
    {
        var config = Config(new()
        {
            ["Jwt:Issuer"] = "my-issuer",
            ["Jwt:Audience"] = "my-audience"
        });
        using var provider = BuildProvider(config);

        var parameters = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;

        Assert.Equal("my-issuer", parameters.ValidIssuer);
        Assert.Equal("my-audience", parameters.ValidAudience);
    }

    [Fact]
    public void AddApiServices_JwtBearerParams_AreCompatibleWithJwtTokenService()
    {
        // Сквозная проверка: токен, выпущенный JwtTokenService на тех же значениях конфига,
        // проходит валидацию параметрами, которые AddApiServices отдал в JwtBearer
        var config = Config();
        using var provider = BuildProvider(config);

        var token = new Kontursvet.Infrastructure.Security.JwtTokenService(config)
            .CreateToken(new Kontursvet.Domain.Entities.AdminUser
            {
                Id = 1,
                Username = "admin",
                Role = "Admin"
            });

        var parameters = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };
        var principal = handler.ValidateToken(token, parameters, out _);

        Assert.Equal("1", principal.FindFirst(System.IdentityModel.Tokens.Jwt
            .JwtRegisteredClaimNames.Sub)!.Value);
        Assert.True(principal.IsInRole("Admin"));
    }

    [Fact]
    public void AddApiServices_JwtBearerParams_RejectTokenSignedWithAnotherSecret()
    {
        var config = Config();
        using var provider = BuildProvider(config);

        var foreignToken = new Kontursvet.Infrastructure.Security.JwtTokenService(
                Config(new() { ["Jwt:SecretKey"] = "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ" }))
            .CreateToken(new Kontursvet.Domain.Entities.AdminUser { Id = 1, Username = "admin" });

        var parameters = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;

        Assert.ThrowsAny<Microsoft.IdentityModel.Tokens.SecurityTokenException>(() =>
            new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ValidateToken(foreignToken, parameters, out _));
    }

    [Fact]
    public void AddApiServices_MissingIssuerAndAudience_LeavesThemNull()
    {
        var config = Config(new() { ["Jwt:Issuer"] = null, ["Jwt:Audience"] = null });
        using var provider = BuildProvider(config);

        var parameters = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;

        // Валидация включена, а ожидаемых значений нет → живой токен не пройдёт проверку.
        // Фиксируем, что проброс значений из конфига действительно используется.
        Assert.True(parameters.ValidateIssuer);
        Assert.Null(parameters.ValidIssuer);
        Assert.Null(parameters.ValidAudience);
    }

    // ==================== Посев администратора ====================

    [Fact]
    public void AddApiServices_RegistersAdminUserSeederAsHostedService()
    {
        var services = new ServiceCollection();

        services.AddApiServices(Config());

        var descriptor = Assert.Single(
            services,
            d => d.ServiceType == typeof(IHostedService)
                 && d.ImplementationType == typeof(AdminUserSeeder));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    // ==================== JSON ====================

    [Fact]
    public void AddApiServices_UsesCamelCaseJsonNaming()
    {
        using var provider = BuildProvider(Config());

        var jsonOptions = provider
            .GetRequiredService<IOptionsMonitor<Microsoft.AspNetCore.Http.Json.JsonOptions>>()
            .CurrentValue;

        Assert.Same(System.Text.Json.JsonNamingPolicy.CamelCase,
            jsonOptions.SerializerOptions.PropertyNamingPolicy);
    }

    // ==================== Swagger ====================

    [Fact]
    public void AddApiServices_RegistersSwaggerServices()
    {
        var services = new ServiceCollection();

        services.AddApiServices(Config());

        // Сам ISwaggerProvider резолвится только на реальном хосте (нужен EndpointDataSource),
        // поэтому здесь проверяем лишь, что AddEndpointsApiExplorer + AddSwaggerGen внесли свои сервисы
        Assert.Contains(services, d =>
            d.ServiceType.FullName!.Contains("ISwaggerProvider", StringComparison.Ordinal));
        Assert.Contains(services, d =>
            d.ServiceType.FullName!.Contains("IApiDescriptionGroupCollectionProvider", StringComparison.Ordinal));
    }

    [Fact]
    public void AddApiServices_RegistersAuthorizationServices()
    {
        using var provider = BuildProvider(Config());

        Assert.NotNull(provider.GetService<Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider>());
    }

    [Fact]
    public void AddApiServices_RegistersCorsWithDefaultPolicy()
    {
        using var provider = BuildProvider(Config());

        var options = provider
            .GetRequiredService<IOptionsMonitor<Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions>>()
            .CurrentValue;

        Assert.NotNull(options.GetPolicy(options.DefaultPolicyName));
    }
}
