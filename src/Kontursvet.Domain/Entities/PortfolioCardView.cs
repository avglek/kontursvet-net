namespace Kontursvet.Domain.Entities;

public sealed class PortfolioCardView
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // ex-"case" — переименовано, т.к. "case" — ключевое слово C#
    public string Part { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Task { get; set; } = string.Empty;
    public List<string> Works { get; set; } = [];
    public string Location { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Features { get; set; } = string.Empty;
    public List<string> Meta { get; set; } = [];
    public List<GalleryItem> Photos { get; set; } = [];
}