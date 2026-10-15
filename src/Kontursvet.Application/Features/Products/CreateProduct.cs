using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Products;

public sealed record CreateProductCommand(ProductCardDto Card);

public sealed class CreateProductHandler(IProductRepository repository)
{
    public async Task<Result<long>> HandleAsync(CreateProductCommand cmd, CancellationToken ct)
    {
        var dto = cmd.Card;

        if (string.IsNullOrWhiteSpace(dto.Code))
            return Result<long>.Failure("Поле Code обязательно", "PRODUCT_CODE_REQUIRED");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result<long>.Failure("Поле Name обязательно", "PRODUCT_NAME_REQUIRED");

        var existing = await repository.GetByCodeAsync(dto.Code, ct);
        if (existing is not null)
            return Result<long>.Failure($"Товар с кодом '{dto.Code}' уже существует", "PRODUCT_CODE_DUPLICATE");

        var id = await repository.CreateAsync(dto.ToDomain(), ct);
        return Result<long>.Success(id);
    }
}
