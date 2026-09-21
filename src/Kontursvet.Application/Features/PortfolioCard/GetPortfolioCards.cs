using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.PortfolioCard;

public sealed record GetPortfolioCardsQuery(int Skip, int Take);

public sealed class GetPortfolioCardsHandler(IPortfolioCardRepository repository)
{
    private const int MaxTake = 200;

    public async Task<Result<IReadOnlyList<PortfolioCardDto>>> HandleAsync(
        GetPortfolioCardsQuery query, CancellationToken ct)
    {
        var skip = Math.Max(0, query.Skip);
        var take = Math.Clamp(query.Take, 1, MaxTake);

        var items = await repository.GetAllAsync(skip, take, ct);
        var dtos = items.Select(x => x.ToDto()).ToList();

        return Result<IReadOnlyList<PortfolioCardDto>>.Success(dtos);
    }
}