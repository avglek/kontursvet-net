using Kontursvet.Domain.Entities;

namespace Kontursvet.Application.Abstractions;

public interface IPortfolioRepository
{
    // Card (превью)
    Task<PortfolioCard?> GetCardByIdAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<PortfolioCard>> GetAllCardsAsync(int skip, int take, CancellationToken ct);
    Task<long> CreateCardAsync(PortfolioCard card, CancellationToken ct);
    Task<bool> UpdateCardAsync(PortfolioCard card, CancellationToken ct);
    Task<bool> DeleteCardAsync(long id, CancellationToken ct);
    Task<PortfolioCardCount?> GetCardsCountAsync(CancellationToken ct);

    // CardView (представление)
    Task<PortfolioCardView?> GetViewByIdAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<PortfolioCardView>> GetAllViewsAsync(int skip, int take, CancellationToken ct);
    Task<bool> UpsertViewAsync(PortfolioCardView view, CancellationToken ct);
    Task<bool> DeleteViewAsync(long id, CancellationToken ct);

    // Агрегат
    Task<(PortfolioCard? Card, PortfolioCardView? View)> GetFullAsync(long id, CancellationToken ct);

    // Галерея — атомарные операции
    Task<bool> AddGalleryItemAsync(long cardViewId, GalleryItem item, CancellationToken ct);
    Task<bool> UpdateGalleryItemAsync(long cardViewId, int key, GalleryItem item, CancellationToken ct);
    Task<bool> DeleteGalleryItemAsync(long cardViewId, int key, CancellationToken ct);
    Task<bool> ReplaceGalleryAsync(long cardViewId, IReadOnlyList<GalleryItem> gallery, CancellationToken ct);
}