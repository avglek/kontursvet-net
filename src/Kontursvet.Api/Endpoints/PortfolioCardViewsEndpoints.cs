using Kontursvet.Api.Common;
using Kontursvet.Application.Dtos;
using Kontursvet.Application.Features.Portfolio.CardView;

namespace Kontursvet.Api.Endpoints;

public static class PortfolioCardViewsEndpoints
{
    public static IEndpointRouteBuilder MapPortfolioCardViewsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profile/cardviews").WithTags("ProfileCardViews");

        // GET /api/profile/cardviews?skip=0&take=20
        group.MapGet("/", async (
            GetPortfolioCardViewsHandler handler,
            int? skip,
            int? take,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(
                new GetPortfolioCardViewsQuery(skip ?? 0, take ?? 20), ct);
            return result.ToHttp();
        })
        .WithName("GetPortfolioCardViews")
        .Produces<IReadOnlyList<PortfolioCardViewDto>>(StatusCodes.Status200OK)
        .AllowAnonymous();

        // GET /api/profile/cardveiws{id}
        group.MapGet("/{id:long}", async (
            long id,
            GetPortfolioCardViewHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetPortfolioCardViewQuery(id), ct);
            return result.ToHttp();
        })
        .WithName("GetPortfolioCardView")
        .Produces<PortfolioCardViewDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AllowAnonymous();

        // PUT /api/profile/cardviews{id}
        group.MapPut("/{id:long}", async (
            long id,
            PortfolioCardViewDto dto,
            UpdatePortfolioCardViewHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new UpdatePortfolioCardViewCommand(id, dto), ct);
            return result.ToHttp();
        })
        .WithName("UpdatePortfolioCardView")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // DELETE /api/profile/cardviews{id}
        group.MapDelete("/{id:long}", async (
            long id,
            DeletePortfolioCardViewHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new DeletePortfolioCardViewCommand(id), ct);
            return result.ToHttp();
        })
        .WithName("DeletePortfolioCardView")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        return app;
    }
}