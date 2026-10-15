using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Products;

public sealed record GetProductByIdQuery(long Id);

public sealed class GetProductByIdHandler(IProductRepository repository)
{
    public async Task<Result<ProductCardDto>> HandleAsync(GetProductByIdQuery query, CancellationToken ct)
    {
        var card = await repository.GetByIdAsync(query.Id, ct);
        if (card is null)
            return Result<ProductCardDto>.Failure("Товар не найден", "PRODUCT_NOT_FOUND");

        return Result<ProductCardDto>.Success(card.ToDto());
    }
}
