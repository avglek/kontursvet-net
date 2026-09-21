using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Portfolio.Gallery;

public sealed record DeleteGalleryItemCommand(long CardViewId, int Key);

public sealed class DeleteGalleryItemHandler(IPortfolioRepository repository)
{
    public async Task<Result> HandleAsync(DeleteGalleryItemCommand cmd, CancellationToken ct)
    {
        var view = await repository.GetViewByIdAsync(cmd.CardViewId, ct);
        if (view is null)
            return Result.Failure("Представление не найдено", "VIEW_NOT_FOUND");

        if (view.Gallery.All(g => g.Key != cmd.Key))
            return Result.Failure($"Элемент с Key={cmd.Key} не найден", "GALLERY_ITEM_NOT_FOUND");

        var deleted = await repository.DeleteGalleryItemAsync(cmd.CardViewId, cmd.Key, ct);

        return deleted
            ? Result.Success()
            : Result.Failure("Не удалось удалить элемент", "GALLERY_DELETE_FAILED");
    }
}