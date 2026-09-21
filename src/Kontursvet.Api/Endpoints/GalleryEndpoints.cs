using Kontursvet.Api.Common;
using Kontursvet.Application.Dtos;
using Kontursvet.Application.Features.Portfolio.Gallery;

namespace Kontursvet.Api.Endpoints;

public static class GalleryEndpoints
{
    public static IEndpointRouteBuilder MapGalleryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/profiles/{cardViewId:long}/view/gallery")
                       .WithTags("Gallery");

        // POST /api/profiles/{cardViewId}/view/gallery
        group.MapPost("/", async (
            long cardViewId,
            GalleryItemDto dto,
            AddGalleryItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(
                new AddGalleryItemCommand(cardViewId, dto), ct);
            return result.ToHttp();
        })
        .WithName("AddGalleryItem")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // PUT /api/profiles/{cardViewId}/view/gallery/{key}
        group.MapPut("/{key:int}", async (
            long cardViewId,
            int key,
            GalleryItemDto dto,
            UpdateGalleryItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(
                new UpdateGalleryItemCommand(cardViewId, key, dto), ct);
            return result.ToHttp();
        })
        .WithName("UpdateGalleryItem")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // DELETE /api/profiles/{cardViewId}/view/gallery/{key}
        group.MapDelete("/{key:int}", async (
            long cardViewId,
            int key,
            DeleteGalleryItemHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(
                new DeleteGalleryItemCommand(cardViewId, key), ct);
            return result.ToHttp();
        })
        .WithName("DeleteGalleryItem")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}