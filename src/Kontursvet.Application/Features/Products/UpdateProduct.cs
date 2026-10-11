using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Products;

public sealed record UpdateProductCommand(long Id, ProductCardDto Card);

public sealed class UpdateProductHandler(IProductRepository repository)
{
    public async Task<Result> HandleAsync(UpdateProductCommand cmd, CancellationToken ct)
    {
        var dto = cmd.Card;

        if (string.IsNullOrWhiteSpace(dto.Code))
            return Result.Failure("Поле Code обязательно", "PRODUCT_CODE_REQUIRED");

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result.Failure("Поле Name обязательно", "PRODUCT_NAME_REQUIRED");

        // Проверяем существование — чтобы отдать 404, а не молчаливый no-op
        var existing = await repository.GetByIdAsync(cmd.Id, ct);
        if (existing is null)
            return Result.Failure("Товар не найден", "PRODUCT_NOT_FOUND");

        // Если меняется код — проверяем уникальность
        if (existing.Code != dto.Code)
        {
            var byCode = await repository.GetByCodeAsync(dto.Code, ct);
            if (byCode is not null)
                return Result.Failure($"Товар с кодом '{dto.Code}' уже существует", "PRODUCT_CODE_DUPLICATE");
        }

        dto.Id = cmd.Id;
        var ok = await repository.UpdateAsync(dto.ToDomain(), ct);

        return ok
            ? Result.Success()
            : Result.Failure("Не удалось обновить товар", "PRODUCT_UPDATE_FAILED");
    }
}
