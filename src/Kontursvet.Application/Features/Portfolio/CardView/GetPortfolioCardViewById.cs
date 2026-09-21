using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Portfolio.CardView;

public sealed record GetPortfolioCardViewQuery(long Id);

public sealed class GetPortfolioCardViewHandler(IPortfolioRepository repository)
{
    public async Task<Result<PortfolioCardViewDto>> HandleAsync(GetPortfolioCardViewQuery query, CancellationToken ct)
    {
        var card = await repository.GetViewByIdAsync(query.Id, ct);
        if (card is null)
            return Result<PortfolioCardViewDto>.Failure("Карточка не найдена", "CARD_NOT_FOUND");

        return Result<PortfolioCardViewDto>.Success(card.ToDto());
    }
}