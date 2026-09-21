using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Portfolio.Card;

public sealed record DeletePortfolioCardCommand(long Id);

public sealed class DeletePortfolioCardHandler(IPortfolioRepository repository)
{
    public async Task<Result> HandleAsync(DeletePortfolioCardCommand cmd, CancellationToken ct)
    {
        var existing = await repository.GetCardByIdAsync(cmd.Id, ct);
        if (existing is null)
            return Result.Failure("Карточка не найдена", "CARD_NOT_FOUND");

        var ok = await repository.DeleteCardAsync(cmd.Id, ct);
        return ok
            ? Result.Success()
            : Result.Failure("Не удалось удалить карточку", "CARD_DELETE_FAILED");
    }
}