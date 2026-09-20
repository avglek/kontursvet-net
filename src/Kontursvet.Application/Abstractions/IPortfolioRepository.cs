using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Abstractions;

public interface IPortfolioRepository
{
    Task<PortfolioCardView?> GetByIdAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<PortfolioCardView>> GetAllAsync(int skip, int take, CancellationToken ct);
    Task<long> CreateAsync(PortfolioCardView card, CancellationToken ct);
    Task<bool> UpdateAsync(PortfolioCardView card, CancellationToken ct);
    Task<bool> DeleteAsync(long id, CancellationToken ct);
}