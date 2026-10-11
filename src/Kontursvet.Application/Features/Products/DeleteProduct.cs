using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Products;

public sealed record DeleteProductCommand(long Id);

public sealed class DeleteProductHandler(IProductRepository repository)
{
    public async Task<Result> HandleAsync(DeleteProductCommand cmd, CancellationToken ct)
    {
        var existing = await repository.GetByIdAsync(cmd.Id, ct);
        if (existing is null)
            return Result.Failure("Товар не найден", "PRODUCT_NOT_FOUND");

        var ok = await repository.DeleteAsync(cmd.Id, ct);
        return ok
            ? Result.Success()
            : Result.Failure("Не удалось удалить товар", "PRODUCT_DELETE_FAILED");
    }
}
