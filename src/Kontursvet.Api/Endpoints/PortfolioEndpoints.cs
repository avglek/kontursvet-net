using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Entities;

namespace Kontursvet.Api.Endpoints;

public static class PortfolioEndpoints
{
    public static IEndpointRouteBuilder MapPortfolioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profiles").WithTags("Profiles");

        // GET /api/profiles?skip=0&take=20
        group.MapGet("/", async (IPortfolioRepository repo, int? skip, int? take, CancellationToken ct) =>
        {
            var items = await repo.GetAllAsync(skip ?? 0, take ?? 20, ct);
            return Results.Ok(items);
        });

        // GET /api/profiles/{id}
        group.MapGet("/{id:long}", async (long id, IPortfolioRepository repo, CancellationToken ct) =>
        {
            var card = await repo.GetByIdAsync(id, ct);
            return card is null ? Results.NotFound() : Results.Ok(card);
        });

        // POST /api/profiles
        group.MapPost("/", async (PortfolioCardView card, IPortfolioRepository repo, CancellationToken ct) =>
        {
            var id = await repo.CreateAsync(card, ct);
            return Results.Created($"/api/profiles/{id}", new { id });
        });

        // PUT /api/profiles/{id}
        group.MapPut("/{id:long}", async (long id, PortfolioCardView card, IPortfolioRepository repo, CancellationToken ct) =>
        {
            card.Id = id;
            var ok = await repo.UpdateAsync(card, ct);
            return ok ? Results.NoContent() : Results.NotFound();
        });

        // DELETE /api/profiles/{id}
        group.MapDelete("/{id:long}", async (long id, IPortfolioRepository repo, CancellationToken ct) =>
        {
            var ok = await repo.DeleteAsync(id, ct);
            return ok ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}