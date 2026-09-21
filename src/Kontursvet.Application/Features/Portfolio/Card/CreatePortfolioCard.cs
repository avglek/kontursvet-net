using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.Portfolio.Card;

public sealed record CreatePortfolioCardCommand(PortfolioCardDto Card);

public sealed class CreatePortfolioCardHandler(IPortfolioRepository repository)
{
    public async Task<Result<long>> HandleAsync(CreatePortfolioCardCommand cmd, CancellationToken ct)
    {
        var dto = cmd.Card;

        if (string.IsNullOrWhiteSpace(dto.Link))
            return Result<long>.Failure("Поле Link обязательно", "CARD_LINK_REQUIRED");

        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<long>.Failure("Поле Title обязательно", "CARD_TITLE_REQUIRED");

        var id = await repository.CreateCardAsync(dto.ToDomain(), ct);
        return Result<long>.Success(id);
    }
}