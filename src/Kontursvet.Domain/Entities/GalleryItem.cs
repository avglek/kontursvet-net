namespace Kontursvet.Domain.Entities;

public sealed class GalleryItem
{
    public int Key { get; set; }
    public string Src { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public string Figcaption { get; set; } = string.Empty;
}