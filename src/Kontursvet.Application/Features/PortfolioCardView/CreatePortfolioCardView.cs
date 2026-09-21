using Kontursvet.Application.Abstractions;
using Kontursvet.Application.Dtos;
using Kontursvet.Domain.Common;

namespace Kontursvet.Application.Features.PortfolioCardView;

public sealed record CreatePortfolioCardViewCommand(PortfolioCardViewDto Card);

public sealed class CreatePortfolioCardViewHandler(IPortfolioCardViewRepository repository)
{
    public async Task<Result<long>> HandleAsync(CreatePortfolioCardViewCommand cmd, CancellationToken ct)
    {
        var dto = cmd.Card;

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Result<long>.Failure("Поле Name обязательно", "CARD_NAME_REQUIRED");

        if (string.IsNullOrWhiteSpace(dto.Part))
            return Result<long>.Failure("Поле Part обязательно", "CARD_PART_REQUIRED");

        if (string.IsNullOrWhiteSpace(dto.Title))
            return Result<long>.Failure("Поле Title обязательно", "CARD_TITLE_REQUIRED");

        var id = await repository.CreateAsync(dto.ToDomain(), ct);
        return Result<long>.Success(id);
    }
}