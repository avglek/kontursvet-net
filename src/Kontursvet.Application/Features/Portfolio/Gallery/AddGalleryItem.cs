using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;
using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Features.Portfolio.Gallery;

public sealed record AddGalleryItemCommand(long CardViewId, GalleryItemDto Item);

public sealed class AddGalleryItemHandler(IPortfolioRepository repository)
{
    public async Task<Result> HandleAsync(AddGalleryItemCommand cmd, CancellationToken ct)
    {
        var item = cmd.Item;

        if (item.Key < 0)
            return Result.Failure("Key не может быть отрицательным", "GALLERY_KEY_INVALID");

        if (string.IsNullOrWhiteSpace(item.Src))
            return Result.Failure("Src обязателен", "GALLERY_SRC_REQUIRED");

        var view = await repository.GetViewByIdAsync(cmd.CardViewId, ct);
        if (view is null)
            return Result.Failure("Представление не найдено", "VIEW_NOT_FOUND");

        // Защита от дубликата Key внутри галереи
        if (view.Gallery.Any(g => g.Key == item.Key))
            return Result.Failure($"Элемент с Key={item.Key} уже существует", "GALLERY_KEY_DUPLICATE");

        var added = await repository.AddGalleryItemAsync(
            cmd.CardViewId,
            new GalleryItem(item.Key, item.Src, item.Alt, item.Figcaption),
            ct);

        return added
            ? Result.Success()
            : Result.Failure("Не удалось добавить элемент", "GALLERY_ADD_FAILED");
    }
}