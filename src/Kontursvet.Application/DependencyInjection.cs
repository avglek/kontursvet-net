using Kontursvet.Application.Features.Leads;
using Kontursvet.Application.Features.PortfolioCard;
using Kontursvet.Application.Features.PortfolioCardView;
using Microsoft.Extensions.DependencyInjection;

namespace Kontursvet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // PortfolioCard
        services.AddScoped<CreatePortfolioCardHandler>();
        services.AddScoped<GetPortfolioCardHandler>();
        services.AddScoped<GetPortfolioCardsHandler>();
        services.AddScoped<UpdatePortfolioCardHandler>();
        services.AddScoped<DeletePortfolioCardHandler>();

        // PortfolioCardView
        services.AddScoped<CreatePortfolioCardViewHandler>();
        services.AddScoped<GetPortfolioCardViewHandler>();
        services.AddScoped<GetPortfolioCardViewsHandler>();
        services.AddScoped<UpdatePortfolioCardViewHandler>();
        services.AddScoped<DeletePortfolioCardViewHandler>();

        // Leads
        services.AddScoped<SendLeadHandler>();

        return services;
    }
}