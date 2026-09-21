using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;
using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Features.Portfolio.Gallery;

public sealed record UpdateGalleryItemCommand(long CardViewId, int Key, GalleryItemDto Item);

public sealed class UpdateGalleryItemHandler(IPortfolioRepository repository)
{
    public async Task<Result> HandleAsync(UpdateGalleryItemCommand cmd, CancellationToken ct)
    {
        var item = cmd.Item;

        if (string.IsNullOrWhiteSpace(item.Src))
            return Result.Failure("Src обязателен", "GALLERY_SRC_REQUIRED");

        // Key в path и в body должны совпадать — иначе непонятно, что менять
        if (item.Key != cmd.Key)
            return Result.Failure("Key в path и body не совпадают", "GALLERY_KEY_MISMATCH");

        var view = await repository.GetViewByIdAsync(cmd.CardViewId, ct);
        if (view is null)
            return Result.Failure("Представление не найдено", "VIEW_NOT_FOUND");

        if (view.Gallery.All(g => g.Key != cmd.Key))
            return Result.Failure($"Элемент с Key={cmd.Key} не найден", "GALLERY_ITEM_NOT_FOUND");

        var updated = await repository.UpdateGalleryItemAsync(
            cmd.CardViewId,
            cmd.Key,
            new GalleryItem(item.Key, item.Src, item.Alt, item.Figcaption),
            ct);

        return updated
            ? Result.Success()
            : Result.Failure("Не удалось обновить элемент", "GALLERY_UPDATE_FAILED");
    }
}