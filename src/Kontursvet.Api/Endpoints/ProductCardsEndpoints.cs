using Kontursvet.Api.Common;
using Kontursvet.Application.Dtos;
using Kontursvet.Application.Features.Products;

namespace Kontursvet.Api.Endpoints;

public static class ProductCardsEndpoints
{
    public static IEndpointRouteBuilder MapProductCardsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/proucts").WithTags("Products");

        // GET /api/products?skip=0&take=20
        group.MapGet("/", async (
            GetAllProductsHandler handler,
            int? skip,
            int? take,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(
                new GetAllProductsQuery(skip ?? 0, take ?? 20), ct);
            return result.ToHttp();
        })
        .WithName("GetAllProducts")
        .Produces<IReadOnlyList<ProductCardDto>>(StatusCodes.Status200OK)
        .AllowAnonymous();


        // GET /api/products/{id}
        group.MapGet("/{id:long}", async (
            long id,
            GetProductByIdHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new GetProductByIdQuery(id), ct);
            return result.ToHttp();
        })
        .WithName("GetProductById")
        .Produces<ProductCardDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AllowAnonymous();

        // POST /api/products
        group.MapPost("/", async (
            ProductCardDto dto,
            CreateProductHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new CreateProductCommand(dto), ct);

            return result.IsSuccess
                ? Results.Created($"/api/profiles/{result.Value}", new { id = result.Value })
                : result.ToHttp();
        })
        .WithName("CreateProduct")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization();

        // PUT /api/products/{id}
        group.MapPut("/{id:long}", async (
            long id,
            ProductCardDto dto,
            UpdateProductHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new UpdateProductCommand(id, dto), ct);
            return result.ToHttp();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        // DELETE /api/products/{id}
        group.MapDelete("/{id:long}", async (
            long id,
            DeleteProductHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(new DeleteProductCommand(id), ct);
            return result.ToHttp();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();

        return app;
    }
}