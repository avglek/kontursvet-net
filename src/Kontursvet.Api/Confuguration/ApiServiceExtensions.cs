using System.Text.Json;

namespace Kontursvet.Configuration;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new()
            {
                Title = "КонтурСвет API",
                Version = "v1",
                Description = "Портфолио CRUD + отправка сообщений в MAX"
            });
        });

        services.AddCors(options =>
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy =
                JsonNamingPolicy.CamelCase;
        });

        return services;
    }
}