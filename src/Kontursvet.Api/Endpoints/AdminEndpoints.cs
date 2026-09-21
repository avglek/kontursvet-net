using System.Security.Claims;
using Kontursvet.Application.Abstractions.Users;

namespace Kontursvet.Api.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin")
                       .RequireAuthorization();

        group.MapGet("/me", async (
            ClaimsPrincipal user,
            IAdminUserRepository repo,
            CancellationToken ct) =>
        {
            var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? user.FindFirst("sub")?.Value;
            if (sub is null || !long.TryParse(sub, out var id))
                return Results.Unauthorized();

            var admin = await repo.GetByIdAsync(id, ct);
            if (admin is null) return Results.Unauthorized();

            return Results.Ok(new { admin.Id, admin.Username, admin.Role });
        })
        .WithName("GetCurrentAdmin");

        return app;
    }
}