using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Portfolio.Card;

public sealed record GetPortfolioCardsCount();

public sealed class GetPortfolioCardsCountHandler(IPortfolioRepository repository)
{
    private const int MaxTake = 200;

    public async Task<Result<PortfolioCardCountDto>> HandleAsync(
        GetPortfolioCardsCount query, CancellationToken ct)
    {

        var count = await repository.GetCardsCountAsync(ct);
        if (count is null)
            return Result<PortfolioCardCountDto>.Failure("Ошибка получения количества карточек", "ERROR_GET_CARDS_COUNT");

        return Result<PortfolioCardCountDto>.Success(count.ToDto());
    }
}