using Kontursvet.Api.Common;
using Kontursvet.Application.Features.Auth;

namespace Kontursvet.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (
            LoginRequest req,
            LoginHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(
                new LoginCommand(req.Username, req.Password), ct);
            return result.ToHttp();
        })
        .WithName("Login")
        .Produces<LoginResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }
}

public sealed record LoginRequest(string Username, string Password);