using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductCard>> GetAllAsync(int skip, int take, CancellationToken ct);
    Task<ProductCard?> GetByIdAsync(long id, CancellationToken ct);
    Task<ProductCard?> GetByCodeAsync(string code, CancellationToken ct);
    Task<long> CreateAsync(ProductCard card, CancellationToken ct);
    Task<bool> UpdateAsync(ProductCard card, CancellationToken ct);
    Task<bool> DeleteAsync(long id, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
}
