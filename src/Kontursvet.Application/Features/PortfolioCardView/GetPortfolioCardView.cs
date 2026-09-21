using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.PortfolioCardView;

public sealed record GetPortfolioCardViewQuery(long Id);

public sealed class GetPortfolioCardViewHandler(IPortfolioCardViewRepository repository)
{
    public async Task<Result<PortfolioCardViewDto>> HandleAsync(GetPortfolioCardViewQuery query, CancellationToken ct)
    {
        var card = await repository.GetByIdAsync(query.Id, ct);
        if (card is null)
            return Result<PortfolioCardViewDto>.Failure("Карточка не найдена", "CARD_NOT_FOUND");

        return Result<PortfolioCardViewDto>.Success(card.ToDto());
    }
}