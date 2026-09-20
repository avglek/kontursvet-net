using Kontursvet.Application.Features.Leads;
using Microsoft.Extensions.DependencyInjection;

namespace Kontursvet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<SendLeadHandler>();
        // остальные handler'ы регистрируем здесь по мере роста
        return services;
    }
}