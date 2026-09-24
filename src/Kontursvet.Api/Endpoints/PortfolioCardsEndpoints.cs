using Kontursvet.Api.Common;
using Kontursvet.Application.Dtos;
using Kontursvet.Application.Features.Portfolio.Card;

namespace Kontursvet.Api.Endpoints;

public static class PortfolioCardsEndpoints
{
    public static IEndpointRouteBuilder MapPortfolioCardsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profile/cards").WithTags("ProfileCards");

        // GET /api/profile/cards?skip=0&take=20
        group.MapGet("/", async (
            GetPortfolioCardsHandler handler,
            int? skip,
            int? take,
            CancellationToken ct) =>
        {
            await Task.Delay(3000);
            var result = await handler.HandleAsync(
                new GetPortfolioCardsQuery(skip ?? 0, take ?? 20), ct);
            return result.ToHttp();
        })
        .WithName("GetPortfolioCards")
        .Produces<IReadOnlyList<PortfolioCardViewDto>>(StatusCodes.Status200OK)
        .AllowAnonymous();

        // GET /api/profile/cards{id}
        group.MapGet("/{id:long}", async (
            long id,
            GetPortfolioCardHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetPortfolioCardQuery(id), ct);
            return result.ToHttp();
        })
        .WithName("GetPortfolioCard")
        .Produces<PortfolioCardViewDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AllowAnonymous();

        // POST /api/profile/cards
        group.MapPost("/", async (
            PortfolioCardDto dto,
            CreatePortfolioCardHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new CreatePortfolioCardCommand(dto), ct);

            return result.IsSuccess
                ? Results.Created($"/api/profiles/{result.Value}", new { id = result.Value })
                : result.ToHttp();
        })
        .WithName("CreatePortfolioCard")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization();

        // PUT /api/profile/cards/{id}
        group.MapPut("/{id:long}", async (
            long id,
            PortfolioCardDto dto,
            UpdatePortfolioCardHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new UpdatePortfolioCardCommand(id, dto), ct);
            return result.ToHttp();
        })
        .WithName("UpdatePortfolioCard")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // DELETE /api/profile/cards/{id}
        group.MapDelete("/{id:long}", async (
            long id,
            DeletePortfolioCardHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new DeletePortfolioCardCommand(id), ct);
            return result.ToHttp();
        })
        .WithName("DeletePortfolioCard")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        return app;
    }
}