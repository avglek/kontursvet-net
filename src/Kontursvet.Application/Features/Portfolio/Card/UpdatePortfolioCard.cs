using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Portfolio.Card;

public sealed record UpdatePortfolioCardCommand(long Id, PortfolioCardDto Card);

public sealed class UpdatePortfolioCardHandler(IPortfolioRepository repository)
{
    public async Task<Result> HandleAsync(UpdatePortfolioCardCommand cmd, CancellationToken ct)
    {
        var dto = cmd.Card;

        if (string.IsNullOrWhiteSpace(dto.Link))
            return Result.Failure("Поле Name обязательно", "CARD_NAME_REQUIRED");

        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result.Failure("Поле Title обязательно", "CARD_TITLE_REQUIRED");

        // Проверяем существование — чтобы отдать 404, а не молчаливый no-op
        var existing = await repository.GetCardByIdAsync(cmd.Id, ct);
        if (existing is null)
            return Result.Failure("Карточка не найдена", "CARD_NOT_FOUND");

        dto.Id = cmd.Id;
        var ok = await repository.UpdateCardAsync(dto.ToDomain(), ct);

        return ok
            ? Result.Success()
            : Result.Failure("Не удалось обновить карточку", "CARD_UPDATE_FAILED");
    }
}