using Kontursvet.Application.Features.Auth;
using Kontursvet.Application.Features.Leads;
using Kontursvet.Application.Features.Portfolio.Card;
using Kontursvet.Application.Features.Portfolio.CardView;
using Kontursvet.Application.Features.Portfolio.Gallery;
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
        services.AddScoped<GetPortfolioCardViewHandler>();
        services.AddScoped<GetPortfolioCardViewsHandler>();
        services.AddScoped<UpdatePortfolioCardViewHandler>();
        services.AddScoped<DeletePortfolioCardViewHandler>();

        // Галерея
        services.AddScoped<AddGalleryItemHandler>();
        services.AddScoped<UpdateGalleryItemHandler>();
        services.AddScoped<DeleteGalleryItemHandler>();

        // Leads
        services.AddScoped<SendLeadHandler>();

        // Auth
        services.AddScoped<LoginHandler>();


        return services;
    }
}