using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.PortfolioCard;

public sealed record GetPortfolioCardQuery(long Id);

public sealed class GetPortfolioCardHandler(IPortfolioCardRepository repository)
{
    public async Task<Result<PortfolioCardDto>> HandleAsync(GetPortfolioCardQuery query, CancellationToken ct)
    {
        var card = await repository.GetByIdAsync(query.Id, ct);
        if (card is null)
            return Result<PortfolioCardDto>.Failure("Карточка не найдена", "CARD_NOT_FOUND");

        return Result<PortfolioCardDto>.Success(card.ToDto());
    }
}