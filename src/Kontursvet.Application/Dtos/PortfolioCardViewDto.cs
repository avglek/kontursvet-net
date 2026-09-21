namespace Kontursvet.Application.Dtos;

/// <summary>
/// DTO для передачи PortfolioCardView между слоями.
/// Отделён от домена, чтобы JSON-контракт не зависел от доменных сущностей.
/// </summary>
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
    public List<GalleryItemDto> Photos { get; set; } = [];
}

public sealed class GalleryItemDto
{
    public int Key { get; set; }
    public string Src { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public string Figcaption { get; set; } = string.Empty;
}