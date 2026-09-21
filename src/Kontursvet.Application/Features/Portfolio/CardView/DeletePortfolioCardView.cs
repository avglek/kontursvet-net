using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Portfolio.CardView;

public sealed record DeletePortfolioCardViewCommand(long Id);

public sealed class DeletePortfolioCardViewHandler(IPortfolioRepository repository)
{
    public async Task<Result> HandleAsync(DeletePortfolioCardViewCommand cmd, CancellationToken ct)
    {
        var existing = await repository.GetViewByIdAsync(cmd.Id, ct);
        if (existing is null)
            return Result.Failure("Карточка не найдена", "CARD_NOT_FOUND");

        var ok = await repository.DeleteViewAsync(cmd.Id, ct);
        return ok
            ? Result.Success()
            : Result.Failure("Не удалось удалить карточку", "CARD_DELETE_FAILED");
    }
}