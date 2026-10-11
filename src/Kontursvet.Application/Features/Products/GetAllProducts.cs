using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Products;

public sealed record GetAllProductsQuery(int Skip, int Take);

public sealed class GetAllProductsHandler(IProductRepository repository)
{
    private const int MaxTake = 200;

    public async Task<Result<IReadOnlyList<ProductCardDto>>> HandleAsync(
        GetAllProductsQuery query, CancellationToken ct)
    {
        var skip = Math.Max(0, query.Skip);
        var take = Math.Clamp(query.Take, 1, MaxTake);

        var items = await repository.GetAllAsync(skip, take, ct);
        var dtos = items.Select(x => x.ToDto()).ToList();

        return Result<IReadOnlyList<ProductCardDto>>.Success(dtos);
    }
}
