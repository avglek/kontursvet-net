namespace Kontursvet.Application.Dtos;

public sealed class PortfolioCardViewDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
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
    public List<GalleryItemDto> Gallery { get; set; } = [];
}

public sealed class GalleryItemDto
{
    public int Key { get; set; }
    public string Src { get; set; } = string.Empty;          // путь до картинки
    public string Alt { get; set; } = string.Empty;          // описание картинки
    public string Figcaption { get; set; } = string.Empty;   // описание вида
}