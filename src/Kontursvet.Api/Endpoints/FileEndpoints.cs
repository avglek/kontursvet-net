using Kontursvet.Api.Common;
using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Features.Files;

namespace Kontursvet.Api.Endpoints;

public static class FileEndpoints
{
    public static IEndpointRouteBuilder MapFileEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/files")
                       .WithTags("Files").RequireAuthorization();

        group.MapPost("/upload", async (
            IFormFile file,
            UploadFileHandler handler,
            CancellationToken ct) =>
        {
            await using var stream = file.OpenReadStream();

            var result = await handler.HandleAsync(new UploadFileCommand(
                stream,
                file.FileName,
                file.ContentType,
                file.Length), ct);

            return result.ToHttp();
        })
        .WithName("UploadFile")
        .DisableAntiforgery()   // для multipart-загрузки через Minimal API
        .Produces<StoredFile>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        // DELETE /api/files?url=/uploads/2026/09/xxx.jpg
        group.MapDelete("/", async (
            string url,
            IFileStorage storage,
            CancellationToken ct) =>
        {
            var ok = await storage.DeleteAsync(url, ct);
            return ok ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteFile")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}