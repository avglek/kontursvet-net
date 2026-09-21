using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Abstractions.Users;
using Kontursvet.Infrastructure.Data;
using Kontursvet.Infrastructure.Messaging;
using Kontursvet.Infrastructure.Repositories;
using Kontursvet.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Kontursvet.Infrastructure.Storage;

namespace Kontursvet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Postgres")
                 ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured");

        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(cs));

        services.AddScoped<IPortfolioRepository, PortfolioRepository>();

        services.AddScoped<ILeadRepository, LeadRepository>();

        // Заменить одной строкой на TelegramDispatcher, когда понадобится
        services.AddScoped<IMessageDispatcher, NoOpMessageDispatcher>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();


        services.AddSingleton<IFileStorage, LocalFileStorage>();

        return services;
    }
}