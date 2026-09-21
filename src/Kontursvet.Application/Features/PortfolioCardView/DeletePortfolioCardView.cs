using Kontursvet.Application.Abstractions;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.PortfolioCardView;

public sealed record DeletePortfolioCardViewCommand(long Id);

public sealed class DeletePortfolioCardViewHandler(IPortfolioCardViewRepository repository)
{
    public async Task<Result> HandleAsync(DeletePortfolioCardViewCommand cmd, CancellationToken ct)
    {
        var existing = await repository.GetByIdAsync(cmd.Id, ct);
        if (existing is null)
            return Result.Failure("Карточка не найдена", "CARD_NOT_FOUND");

        var ok = await repository.DeleteAsync(cmd.Id, ct);
        return ok
            ? Result.Success()
            : Result.Failure("Не удалось удалить карточку", "CARD_DELETE_FAILED");
    }
}