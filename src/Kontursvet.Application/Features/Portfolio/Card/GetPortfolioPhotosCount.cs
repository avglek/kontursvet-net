using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Portfolio.Card;

public sealed record GetPortfolioPhotosCount();

public sealed class GetPortfolioPhotosCountHandler(IPortfolioRepository repository)
{
    public async Task<Result<PortfolioPhotosCountDto>> HandleAsync(
        GetPortfolioPhotosCount query, CancellationToken ct)
    {
        var photos = await repository.GetPhotosCountAsync(ct);
        if (photos is null)
            return Result<PortfolioPhotosCountDto>.Failure("Ошибка получения количества фотографий", "ERROR_GET_PHOTOS_COUNT");

        return Result<PortfolioPhotosCountDto>.Success(new PortfolioPhotosCountDto { Photos = photos.Value });
    }
}
