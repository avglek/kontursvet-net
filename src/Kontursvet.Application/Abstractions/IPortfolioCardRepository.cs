using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Abstractions;

public interface IPortfolioCardRepository
{
    Task<PortfolioCard?> GetByIdAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<PortfolioCard>> GetAllAsync(int skip, int take, CancellationToken ct);
    Task<long> CreateAsync(PortfolioCard card, CancellationToken ct);
    Task<bool> UpdateAsync(PortfolioCard card, CancellationToken ct);
    Task<bool> DeleteAsync(long id, CancellationToken ct);
}