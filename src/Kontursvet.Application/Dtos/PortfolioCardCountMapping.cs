using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Dtos;

public static class PortfolioCardCountMapping
{
    public static PortfolioCardCount ToDomain(this PortfolioCardCountDto dto) => new()
    {
        Quantity = dto.Quantity,
        Last = dto.Last
    };

    public static PortfolioCardCountDto ToDto(this PortfolioCardCount e) => new()
    {
        Quantity = e.Quantity,
        Last = e.Last
    };
}