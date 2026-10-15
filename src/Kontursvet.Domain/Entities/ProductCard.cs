namespace Kontursvet.Domain.Entities;

public sealed class ProductCard
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImgSrc { get; set; }
    public bool IsActive { get; set; } = true;
}
