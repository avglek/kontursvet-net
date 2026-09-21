namespace Kontursvet.Domain.Entities;

public sealed class PortfolioPhoto
{
    public long Id { get; set; }
    public string Part { get; set; } = string.Empty;
    public long CardViewId { get; set; }
    public List<GalleryItem> Gallery { get; set; } = [];
}