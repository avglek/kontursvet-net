using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Portfolio.CardView;

public sealed record UpdatePortfolioCardViewCommand(long Id, PortfolioCardViewDto Card);

public sealed class UpdatePortfolioCardViewHandler(IPortfolioRepository repository)
{
    public async Task<Result> HandleAsync(UpdatePortfolioCardViewCommand cmd, CancellationToken ct)
    {
        var dto = cmd.Card;

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result.Failure("Поле Name обязательно", "CARD_NAME_REQUIRED");

        if (string.IsNullOrWhiteSpace(dto.Part))
            return Result.Failure("Поле Part обязательно", "CARD_PART_REQUIRED");

        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result.Failure("Поле Title обязательно", "CARD_TITLE_REQUIRED");

        // Представление связано 1:1 с карточкой (FK по id), поэтому для нового объекта
        // представления ещё нет. Проверяем существование КАРТОЧКИ — и PUT работает как upsert
        // (UpsertViewAsync делает INSERT ... ON CONFLICT DO UPDATE).
        var card = await repository.GetCardByIdAsync(cmd.Id, ct);
        if (card is null)
            return Result.Failure("Карточка не найдена", "CARD_NOT_FOUND");

        dto.Id = cmd.Id;
        var ok = await repository.UpsertViewAsync(dto.ToDomain(), ct);

        return ok
            ? Result.Success()
            : Result.Failure("Не удалось сохранить представление", "CARD_UPDATE_FAILED");
    }
}
